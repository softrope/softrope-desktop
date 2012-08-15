using System;
using System.Xml.Serialization;
using Un4seen.Bass;

namespace Teknohippy.Softrope
{
    [Serializable()]
    public class Sample
    {
        public Sample()
        {
            Name = "No sample loaded...";
            Volume = 1;
            Weight = 1;
        }

        #region SoundFile Members

        private string fileName;

        private float volume;

        #endregion SoundFile Members

        #region SounfFile Properties

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnLoadingFile(EventArgs.Empty);
            }
        }

        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                Bass.BASS_ChannelSetAttribute(this.Channel, BASSAttribute.BASS_ATTRIB_VOL, volume);
            }
        }

        [XmlIgnoreAttribute]
        public int Channel { get; set; }

        public float Weight { get; set; }        

        public string Name { get; set; }

        #endregion SoundFile Properties

        public event EventHandler LoadingFile;

        protected virtual void OnLoadingFile(EventArgs e)
        {
            if (LoadingFile != null)
            {
                LoadingFile(this, e);
            }
        }
    }
}
