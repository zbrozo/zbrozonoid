using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace zbrozonoid
{
    public class SharePadPosition : IDisposable
    {
        private readonly MemoryMappedFile file;

        public SharePadPosition()
        {
            file = MemoryMappedFile.CreateFromFile("/tmp/zbrozonoid", FileMode.OpenOrCreate, "pad");
        }

        public void Write()
        {
        }

        public void Dispose()
        {
            file?.Dispose();
        }
    }
}
