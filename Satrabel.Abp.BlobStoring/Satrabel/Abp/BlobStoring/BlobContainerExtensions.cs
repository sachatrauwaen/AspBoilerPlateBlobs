using Abp.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Satrabel.AspBoilerPlate.BlobStoring
{
    public static class BlobContainerExtensions
    {
        public static async Task SaveAsync(
            this IBlobContainer container,
            string name,
            byte[] bytes,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                await container.SaveAsync(
                    name,
                    memoryStream,
                    overrideExisting,
                    cancellationToken
                );
            }
        }
        
        public static async Task<byte[]> GetAllBytesAsync(
            this IBlobContainer container,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = await container.GetAsync(name, cancellationToken))
            {
                //return await stream.GetAllBytesAsync(cancellationToken);
                return await ReadToEndAsync(stream, cancellationToken);
            }
        }
        
        public static async Task<byte[]> GetAllBytesOrNullAsync(
            this IBlobContainer container,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var stream = await container.GetOrNullAsync(name, cancellationToken);
            if (stream == null)
            {
                return null;
            }
            
            using (stream)
            {
                //return await stream.GetAllBytesAsync(cancellationToken);
                return await ReadToEndAsync(stream, cancellationToken);
            }
        }

        public static async Task<byte[]> ReadToEndAsync(System.IO.Stream stream, CancellationToken cancellationToken)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead, cancellationToken)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        #region sync

        public static Stream Get(this IBlobContainer container,string name)
        {
            return AsyncHelper.RunSync(()=> container.GetAsync(name));
        }
        public static List<string> GetList(this IBlobContainer container, string prefix)
        {
            return AsyncHelper.RunSync(() => container.GetListAsync(prefix));
        }
        public static void Save(this IBlobContainer container,
            string name,
            Stream stream,
            bool overrideExisting = false)
        {
            AsyncHelper.RunSync(() => container.SaveAsync(name, stream, overrideExisting));
        }

        public static void Save(this IBlobContainer container,
            string name,
            byte[] bytes,
            bool overrideExisting = false)
        {
            AsyncHelper.RunSync(() => container.SaveAsync(name, bytes, overrideExisting));
        }

        public static bool Exists(this IBlobContainer container,
            string name)
        {
            return AsyncHelper.RunSync(() => container.ExistsAsync(name));
        }

        public static bool Delete(this IBlobContainer container, string name)
        {
            return AsyncHelper.RunSync(() => container.DeleteAsync(name));
        }

        public static byte[] GetAllBytes(
            this IBlobContainer container,
            string name)
        {
            return AsyncHelper.RunSync(() => container.GetAllBytesAsync(name));
        }
        #endregion
    }
}