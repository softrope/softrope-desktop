using System;
using System.Collections.Generic;
using Un4seen.Bass;
using System.ComponentModel;
using Un4seen.Bass.AddOn.Wma;
using Un4seen.Bass.AddOn.Mix;
using System.Xml.Serialization;

namespace Teknohippy.Softrope
{
    [Serializable()]
    public class SoundEffect : INotifyPropertyChanged
    {
        public SoundEffect()
        {
            IsLooping = false;
            LoopGap = 0;
            LoopGapVariance = 0;
            gapTimer.Interval = 1;
            SoundEffectVolume = 1;
            gapTimer.Elapsed += new System.Timers.ElapsedEventHandler(gapTimer_Elapsed);
            Samples.ListChanged += new ListChangedEventHandler(Samples_ListChanged);
            endSync = new SYNCPROC(EndSync);
            Name = "Unnamed Sound Effect";
            SoundEffectMixerChannel = BassMix.BASS_Mixer_StreamCreate(44100, 2,
                BASSFlag.BASS_MIXER_NONSTOP |
                BASSFlag.BASS_STREAM_DECODE |
                BASSFlag.BASS_SAMPLE_FLOAT);
            BassMix.BASS_Mixer_ChannelFlags(SoundEffectMixerChannel, BASSFlag.BASS_MIXER_NORAMPIN, BASSFlag.BASS_MIXER_NORAMPIN);
        }

        [XmlIgnoreAttribute]
        public Int32 SoundEffectMixerChannel { get; set; }

        void Samples_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged("Samples");
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                Samples[e.NewIndex].LoadingFile += new EventHandler(Sample_LoadingFile);
                LoadSample(Samples[e.NewIndex]);
            }
        }

        void Sample_LoadingFile(object sender, EventArgs e)
        {
            Sample sample = sender as Sample;
            LoadSample(sample);
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event EventHandler Playing;

        protected virtual void OnPlaying(EventArgs e)
        {
            if (Playing != null)
            {
                Playing(this, e);
            }
        }

        public event EventHandler Stopping;

        protected virtual void OnStopping(EventArgs e)
        {
            if (Stopping != null)
            {
                Stopping(this, e);
            }
        }

        public event EventHandler RequestingStop;

        protected virtual void OnRequestingStop(EventArgs e)
        {
            if (RequestingStop != null)
            {
                RequestingStop(this, e);
            }
        }

        public event EventHandler Ended;

        protected virtual void OnEnded(EventArgs e)
        {
            if (Ended != null)
            {
                Ended(this, e);
            }
        }

        #region SoundEffect Members

        private float volume;

        private bool mute;

        private bool isLooping;

        private bool preDelay;

        private int loopGap;

        private int loopGapVariance;

        [NonSerialized()]
        private SYNCPROC endSync;

        [NonSerialized()]
        private System.Timers.Timer gapTimer = new System.Timers.Timer();

        private int level;

        private string name;

        private BindingList<Sample> samples = new BindingList<Sample>();

        private bool isPlayList;

        #endregion SoundEffect Members

        public Sample CurrentSample { get; set; }        

        #region SoundEffect Properties

        public float SoundEffectVolume
        {
            get { return volume; }
            set
            {
                volume = value;
                Bass.BASS_ChannelSetAttribute(SoundEffectMixerChannel, BASSAttribute.BASS_ATTRIB_VOL, this.SoundEffectVolume);
            }
        }

        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                switch (value)
                {
                    case true:
                        Bass.BASS_ChannelSetAttribute(SoundEffectMixerChannel, BASSAttribute.BASS_ATTRIB_VOL, 0);
                        break;
                    case false:
                        Bass.BASS_ChannelSetAttribute(SoundEffectMixerChannel, BASSAttribute.BASS_ATTRIB_VOL, this.volume);
                        break;
                }
            }
        }

        public bool IsLooping
        {
            get { return isLooping; }
            set
            {
                if (isLooping != value)
                {
                    isLooping = value;
                    SetLoop();
                }
            }
        }

        public bool PreDelay
        {
            get { return preDelay; }
            set { preDelay = value; }
        }

        public int LoopGap
        {
            get { return loopGap; }
            set
            {
                loopGap = value;
                SetLoop();
            }
        }

        public int LoopGapVariance
        {
            get { return loopGapVariance; }
            set
            {
                loopGapVariance = value;
                SetLoop();
            }
        }

        public int Level
        {
            get
            {                
                if (gapTimer.Enabled || CurrentSample == null)
                {
                    return 0;
                }
                int left;
                int right;
                left = Utils.LowWord32(BassMix.BASS_Mixer_ChannelGetLevel(SoundEffectMixerChannel));
                right = Utils.HighWord32(BassMix.BASS_Mixer_ChannelGetLevel(SoundEffectMixerChannel));
                level = (left + right) / 2;
                level = (int)(level / 3276.8);
                return level;

                
            }

        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public BindingList<Sample> Samples
        {
            get { return samples; }
            set { samples = value; }
        }

        public bool IsPlayList
        {
            get { return isPlayList; }
            set { isPlayList = value; }
        }

        #endregion SoundEffect Properties

        #region SoundEffect Methods

        public void LoadSample(Sample sample)
        {
            if (sample.FileName == null) return;
            if (sample.FileName.EndsWith(".wma"))
            {
                sample.Channel = BassWma.BASS_WMA_StreamCreateFile(sample.FileName, 0L, 0L, 
                    BASSFlag.BASS_STREAM_DECODE |
                    BASSFlag.BASS_SAMPLE_FLOAT);
            }
            else
            {
                sample.Channel = Bass.BASS_StreamCreateFile(sample.FileName, 0L, 0L, 
                    BASSFlag.BASS_STREAM_DECODE |
                    BASSFlag.BASS_SAMPLE_FLOAT);
            }

            BassMix.BASS_Mixer_StreamAddChannel(SoundEffectMixerChannel, sample.Channel, 
                BASSFlag.BASS_MIXER_PAUSE);
            Bass.BASS_ChannelSetSync(sample.Channel, BASSSync.BASS_SYNC_END, 0, endSync, IntPtr.Zero);

            //sample.Weight = 1;

            if (CurrentSample == null)
            {
                CurrentSample = sample;
            }
            SetLoop();
        }

        public void RemoveSample(Sample sample)
        {
            BassMix.BASS_Mixer_ChannelRemove(sample.Channel);
            Bass.BASS_StreamFree(sample.Channel);
            Samples.Remove(sample);
        }

        public void Play()
        {
            if (Samples.Count > 0)
            {
                if (this.IsPlayList)
                {
                    CurrentSample = Samples[0];
                }
                else
                {
                    GetNextSoundFile(false);
                }
                
                if (PreDelay)
                {
                    try
                    {
                        gapTimer.Interval = LoopGap + LoopGapVariance;
                        gapTimer.Start();
                    }
                    catch (ArgumentException)
                    {
                        PlayCurrentSample(true);
                    }              
                    
                }
                else
                {
                    PlayCurrentSample(true);
                }

                OnPlaying(EventArgs.Empty);
            }
            else
            {
                //throw new ApplicationException("Could not play sfx. No files loaded");
            }


        }

        public void Stop()
        {
            if (CurrentSample != null)
            {
                BassMix.BASS_Mixer_ChannelPause(CurrentSample.Channel);
            }
            gapTimer.Stop();
            OnStopping(EventArgs.Empty);
            CurrentSample = null;
        }

        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            if (this.IsLooping == true)
            {

                if (this.IsSingleHardLoop == true)
                {
                    //do nothing let it loop
                }
                else // needs to change sample and/or pause
                {
                    BassMix.BASS_Mixer_ChannelPause(CurrentSample.Channel);
                    GetNextSoundFile(IsPlayList);
                    Int32 loopGap = RandomLoopGap();

                    if (loopGap > 0) //needs a gapTimer
                    {
                        gapTimer.Interval = loopGap;
                        gapTimer.Start();
                    }
                    else //no timer needed play next
                    {
                        PlayCurrentSample(true);
                    }
                }
            }
            else
            {
                OnEnded(EventArgs.Empty);
            }
        }

        private void PlayCurrentSample(bool restart)
        {
            if (restart == true)
            {
                BassMix.BASS_Mixer_ChannelSetPosition(CurrentSample.Channel, 0);
            }
            BassMix.BASS_Mixer_ChannelPlay(CurrentSample.Channel);
        }

        public void Next()
        {
            Stop();
            GetNextSoundFile(true);
            Play();
        }

        public void Previous()
        {
            Stop();
            GetPrevSoundFile();
            Play();
        }

        private void GetNextSoundFile(bool sequential)
        {
            if (sequential == true)
            {
                int newIndex = Samples.IndexOf(CurrentSample) + 1;
                if (newIndex >= Samples.Count)
                {
                    newIndex = 0;
                }
                CurrentSample = samples[newIndex];
            }
            else
            {
                float totalWeight = 0;
                Sample winner = CurrentSample;
                float randomNumber;

                foreach (Sample sample in Samples)
                {
                    totalWeight += sample.Weight;
                    randomNumber = (float)Softrope.random.NextDouble() * totalWeight;
                    if (randomNumber < sample.Weight)
                    {
                        winner = sample;
                    }
                }

                CurrentSample = winner;
            }

        }

        private void GetPrevSoundFile()
        {
            int newIndex = Samples.IndexOf(CurrentSample) - 1;
            if (newIndex < 0)
            {
                newIndex = Samples.Count - 1;
            }
            CurrentSample = samples[newIndex];
        }

        private void gapTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (CurrentSample != null)
            {
                gapTimer.Stop();
                PlayCurrentSample(true);
            }
        }

        private int RandomLoopGap()
        {
            return LoopGap + Softrope.random.Next(0, LoopGapVariance);
        }

        private void SetLoop()
        {
            switch (this.IsLooping)
            {
                case true:
                    foreach (Sample sample in Samples)
                    {
                        Bass.BASS_ChannelFlags(sample.Channel, BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                    }
                    break;
                case false:
                    foreach (Sample sample in Samples)
                    {
                        Bass.BASS_ChannelFlags(sample.Channel, BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
                    }
                    break;
            }

        }

        public bool IsSingleHardLoop
        {
            get
            {
                return (LoopGap == 0 && LoopGapVariance == 0 && IsLooping == true && Samples.Count < 2);
            }
        }

        #endregion


        public float TotalWeight
        {
            get
            {
                float total = 0;
                foreach (Sample sample in Samples)
                {
                    total += sample.Weight;
                }
                return total;
            }
        }
        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
