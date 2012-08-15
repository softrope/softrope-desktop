using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using System.Xml.Serialization;

namespace Teknohippy.Softrope
{
    [Serializable()]
    public class Module
    {
        public Module()
        {
            Scenes = new BindingList<Scene>();
            IsDirty = true;
            Scenes.ListChanged += new ListChangedEventHandler(Scenes_ListChanged);
            ModuleMixerChannel = BassMix.BASS_Mixer_StreamCreate(44100, 2,
                BASSFlag.BASS_MIXER_NONSTOP |
                BASSFlag.BASS_SAMPLE_FLOAT);
            BassMix.BASS_Mixer_ChannelFlags(ModuleMixerChannel, BASSFlag.BASS_MIXER_NORAMPIN, BASSFlag.BASS_MIXER_NORAMPIN);

            Bass.BASS_ChannelPlay(ModuleMixerChannel, false);

            reverbChannel = Bass.BASS_ChannelSetFX(ModuleMixerChannel, BASSFXType.BASS_FX_DX8_REVERB, 1000);
            reverb = new BASS_DX8_REVERB();
            reverb.fReverbMix = -96;
            reverb.fReverbTime = 0;
            Bass.BASS_FXSetParameters(reverbChannel, reverb);
        }

        [XmlIgnoreAttribute]
        private Int32 reverbChannel;

        [XmlIgnoreAttribute]
        private BASS_DX8_REVERB reverb;

        [XmlIgnoreAttribute]
        public bool IsDirty { get; set; }

        void Scenes_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                Scenes[e.NewIndex].StoppingOthers += new EventHandler(Module_StoppingOthers);
                BassMix.BASS_Mixer_StreamAddChannel(ModuleMixerChannel, Scenes[e.NewIndex].SceneMixerChannel,
                    BASSFlag.BASS_DEFAULT);
            }
        }

        [XmlIgnore]
        public float ReverbTime
        {
            set
            {
                reverb.fReverbTime = value;
                Bass.BASS_FXSetParameters(reverbChannel, reverb);
            }
            get
            {
                return reverb.fReverbTime;
            }
        }

        [XmlIgnore]
        public float ReverbMix
        {
            set
            {
                reverb.fReverbMix = (float)(Utils.LevelToDB(value, 100.0));
                Bass.BASS_FXSetParameters(reverbChannel, reverb);
            }
            get
            {
                return reverb.fReverbMix;
            }
        }

        void Module_StoppingOthers(object sender, EventArgs e)
        {
            StopAllScenes(sender);
        }

        public BindingList<Scene> Scenes { get; set; }

        [XmlIgnoreAttribute]
        public Int32 ModuleMixerChannel { get; set; }

        public void StopAllScenes(object sender)
        {
            Scene soloScene = sender as Scene;

            foreach (Scene scene in Scenes)
            {
                if (scene.IsPlaying && scene != soloScene)
                    scene.Stop();
            }
        }



    }
}
