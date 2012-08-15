using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace Teknohippy.Softrope
{
    public static class Softrope
    {
        public static bool InitialiseBass()
        {
            BassNet.Registration("iain@norman.org", "2X2931415142415");
            return Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        }

        public static Random random = new Random();


        public static void Save(Module module, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Module));
            FileStream fs = new FileStream(path, FileMode.Create);
            TextWriter writer = new StreamWriter(fs, new UTF8Encoding());
            serializer.Serialize(writer, module);
            writer.Close();
            fs.Close();
        }

        public static Module Load(Module module, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Module));
            FileStream fs = new FileStream(path, FileMode.Open);
            return serializer.Deserialize(fs) as Module;            
        }

        public static void SaveSceneZip(Scene scene, string path)
        {
            using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(path)))
            {
                zipStream.SetLevel(0);

                XmlSerializer serializer = new XmlSerializer(typeof(Scene));
                MemoryStream ms = new MemoryStream();
                TextWriter writer = new StreamWriter(ms, new UTF8Encoding());
                serializer.Serialize(writer, scene);
                writer.Close();

                ZipEntry manifest = new ZipEntry("manifest.xml");

                manifest.DateTime = DateTime.Now;

                zipStream.PutNextEntry(manifest);

                zipStream.Write(ms.ToArray(),0,ms.ToArray().Length);


                foreach (SoundEffect soundEffect in scene.SoundEffects)
                {
                    foreach (Sample sample in soundEffect.Samples)
                    {
                        ZipEntry sampleEntry = new ZipEntry(Path.GetFileName(sample.FileName));
                        zipStream.PutNextEntry(sampleEntry);
                        Byte[] sampleBytes = File.ReadAllBytes(sample.FileName);
                        zipStream.Write(sampleBytes, 0, sampleBytes.Length);
                    }
                }

                zipStream.Finish();
                zipStream.Close();

            }
        }

        public static void UnloadBass()
        {
            Bass.BASS_Free();
            Bass.FreeMe();
        }

        public static void SetBuffer()
        {
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 250);
        }
    }
}
