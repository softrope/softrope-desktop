using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using System.Xml.Serialization;

namespace Teknohippy.Softrope
{
    [Serializable()]
    public class Scene
    {
        const int DEFAULT_FADEOUT_LENGTH = 500;

        public Scene()
        {
            FadeOutLength = DEFAULT_FADEOUT_LENGTH;
            Name = "Unnamed Scene";
            slideSync = new SYNCPROC(SlideSync);
            SoundEffects.ListChanged += new ListChangedEventHandler(SoundEffects_ListChanged);
            SceneMixerChannel = BassMix.BASS_Mixer_StreamCreate(44100, 2,
                BASSFlag.BASS_MIXER_NONSTOP |
                BASSFlag.BASS_STREAM_DECODE |
                BASSFlag.BASS_SAMPLE_FLOAT);
            BassMix.BASS_Mixer_ChannelFlags(SceneMixerChannel, BASSFlag.BASS_MIXER_NORAMPIN, BASSFlag.BASS_MIXER_NORAMPIN);
            Bass.BASS_ChannelSetSync(SceneMixerChannel, BASSSync.BASS_SYNC_SLIDE, 0, slideSync, IntPtr.Zero);

            this.SceneVolume = 1;
            this.IsFading = false;
        }

        void SoundEffects_ListChanged(object sender, ListChangedEventArgs e)
        {
            Stop(false);
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                BassMix.BASS_Mixer_StreamAddChannel(SceneMixerChannel, SoundEffects[e.NewIndex].SoundEffectMixerChannel, 
                    BASSFlag.BASS_DEFAULT);
            }
        }

        private BindingList<SoundEffect> soundEffects = new BindingList<SoundEffect>();

        [XmlIgnoreAttribute]
        public Int32 SceneMixerChannel { get; set; }

        private float sceneVolume = 1;

        private string name;

        public BindingList<SoundEffect> SoundEffects
        {
            get
            {
                return soundEffects;
            }
        }

        public Byte[] ButtonImage { get; set; }


        public float SceneVolume
        {
            get
            {
                return this.sceneVolume;
            }
            set
            {
                this.sceneVolume = value;
                Bass.BASS_ChannelSetAttribute(SceneMixerChannel, BASSAttribute.BASS_ATTRIB_VOL, this.SceneVolume);
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public void Play()
        {
            this.IsPlaying = true;
            if (this.IsFading == true)
            {
                this.IsFading = false;
                Bass.BASS_ChannelSlideAttribute(SceneMixerChannel, BASSAttribute.BASS_ATTRIB_VOL, this.SceneVolume, 1);
            }
            if (StopsOthers)
            {
                OnStoppingOthers(EventArgs.Empty);
            }
            foreach (SoundEffect soundEffect in SoundEffects)
            {
                soundEffect.Play();

            }
            this.IsPlaying = true;
            OnPlaying(EventArgs.Empty);
            
        }

        /// <summary>
        /// Stops scene playing.
        /// </summary>
        public void Stop(bool allowFade)
        {
            if (allowFade == true && FadesOut == true)
            {
                FadeOut();
            }

            else
            {
                IsPlaying = false;

                foreach (SoundEffect soundEffect in SoundEffects)
                {
                    soundEffect.Stop();
                }
                Bass.BASS_ChannelSetAttribute(SceneMixerChannel, BASSAttribute.BASS_ATTRIB_VOL, this.SceneVolume);
                OnStopping(EventArgs.Empty);
            }            
        }

        private void FadeOut()
        {
            this.IsFading = true;
            Bass.BASS_ChannelSlideAttribute(SceneMixerChannel, BASSAttribute.BASS_ATTRIB_VOL, 0, FadeOutLength);            
        }

        private SYNCPROC slideSync;

        private void SlideSync(int handle, int channel, int data, IntPtr user)
        {
            if (this.IsFading == true)
            {
                this.IsFading = false;
                this.Stop(false);
            }
        }

        [XmlIgnoreAttribute]
        public Boolean IsFading { get; set; }

        /// <summary>
        /// Stops scene playing and fades out.
        /// </summary>
        public void Stop()
        {
            Stop(true);
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

        public bool StopsOthers { get; set; }

        public event EventHandler StoppingOthers;

        protected virtual void OnStoppingOthers(EventArgs e)
        {
            if (StoppingOthers != null)
            {
                StoppingOthers(this, e);
            }

        }

        [XmlIgnoreAttribute]
        public bool IsPlaying { get; set; }

        public bool FadesOut { get; set; }

        public int FadeOutLength { get; set; }

    }
}
