using System;
using System.IO;
using Snp.Models;

using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

using Snp.V1;



namespace Snp.Repository.Grpc
{
    /// <summary>
    /// Contains methods for interacting with the customers backend using GRPC. 
    /// </summary>
    public class GrpcMusicUploadRepository : IMusicUploadRepository
    {
        
        private readonly MusicUploadCrud.MusicUploadCrudClient _client;

        public GrpcMusicUploadRepository(CallInvoker invoker)
        {
            _client = new MusicUploadCrud.MusicUploadCrudClient(invoker);
        }
        

        public async Task Upload(string path,string name)
        {
            using var requestStream = _client.UploadMusic();
            const int chunkSize = 512000;
            byte[] fileData;
            Task<int> readAmount;
            await using (var stream = File.Open(path, FileMode.Open))
            {
                fileData = new byte[stream.Length];
                readAmount = stream.ReadAsync(fileData, 0, (int)stream.Length);
                await readAmount;
            }
            
            // byte[] fileData = File.ReadAllBytes(path);
            int totalChunks = (int)Math.Ceiling((double)fileData.Length / chunkSize);

            for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
            {
                int startIndex = chunkIndex * chunkSize;
                int chunkLength = Math.Min(chunkSize, fileData.Length - startIndex);
                byte[] chunk = new byte[chunkLength];
                Array.Copy(fileData, startIndex, chunk, 0, chunkLength);
              await requestStream.RequestStream.WriteAsync(new UploadRequest { FileName = name,ChunkData = ByteString.CopyFrom(chunk) });
            }

            await requestStream.RequestStream.CompleteAsync();

            var finalResponse = await requestStream.ResponseAsync;
            Console.WriteLine($"Upload Complete. Status: {finalResponse.Id}");
        }
    }
}
