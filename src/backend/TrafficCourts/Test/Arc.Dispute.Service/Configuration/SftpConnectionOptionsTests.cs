using Renci.SshNet.Common;
using System;
using System.IO;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using Xunit;

namespace TrafficCourts.Test.Arc.Dispute.Service.Configuration
{
    public class SftpConnectionOptionsTests
    {
        [Theory]
        [InlineData(1024)]
        [InlineData(2048)]
        [InlineData(4096)]
        public void load_private_key_from_string(int keySize)
        {
            // arrange
            var key = new SshKeyGenerator.SshKeyGenerator(keySize);

            SftpConnectionOptions options = new SftpConnectionOptions
            {
                SshPrivateKey = key.ToPrivateKey()
            };

            var privateKey = options.GetPrivateKey();
            Assert.NotNull(privateKey);
        }

        [Theory]
        [InlineData(1024)]
        [InlineData(2048)]
        [InlineData(4096)]
        public void load_private_key_from_file(int keySize)
        {
            string filename = Path.GetTempFileName();

            try
            {
                var key = new SshKeyGenerator.SshKeyGenerator(keySize);
                File.WriteAllText(filename, key.ToPrivateKey());

                SftpConnectionOptions options = new()
                {
                    SshPrivateKeyPath = filename
                };

                var privateKey = options.GetPrivateKey();
                Assert.NotNull(privateKey);
            }
            finally
            {
                TryDelete(filename);
            }
        }

        [Fact]
        public void GetPrivateKey_returns_null_if_keys_not_set()
        {
            SftpConnectionOptions options = new();
            var privateKey = options.GetPrivateKey();
            Assert.Null(privateKey);
        }

        [Fact]
        public void GetPrivateKey_throws_if_SshPrivateKey_is_not_valid()
        {
            SftpConnectionOptions options = new SftpConnectionOptions
            {
                SshPrivateKey = Guid.NewGuid().ToString()
            };

            Assert.Throws<SshException>(() => options.GetPrivateKey());
        }

        [Fact]
        public void GetPrivateKey_throws_if_SshPrivateKeyPath_is_not_valid()
        {
            string filename = Path.GetTempFileName();

            try
            {
                File.WriteAllText(filename, Guid.NewGuid().ToString());

                SftpConnectionOptions options = new SftpConnectionOptions
                {
                    SshPrivateKeyPath = filename
                };

                Assert.Throws<SshException>(() => options.GetPrivateKey());
            }
            finally
            {
                TryDelete(filename);
            }
        }

        private void TryDelete(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception)
            {
            }
        }
    }
}
