using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Lykke.Blockchains.Integrations.Contract.Common.Cryptography
{
    internal static class RsaEncryption
    {
        // encoded OID sequence for PKCS1 RSA "1.2.840.113549.1.1.1"
        private static readonly byte[] Pkcs1Oid = {0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00};

        public static byte[] Decrypt(byte[] encryptedBytes, byte[] privateKey)
        {
            using (var rsa = CreateRsaFromPrivateKey(privateKey))
            {
                var decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

                return decryptedBytes;
            }
        }

        public static byte[] Encrypt(byte[] bytesToEncrypt, byte[] publicKey)
        {
            using (var rsa = CreateRsaFromPublicKey(publicKey))
            {
                return rsa.Encrypt(bytesToEncrypt, RSAEncryptionPadding.Pkcs1);
            }
        }

        private static RSA CreateRsaFromPrivateKey(byte[] privateKey)
        {
            using (var stream = new MemoryStream(privateKey))
            using (var reader = new BinaryReader(stream))
            {
                var twoBytes = reader.ReadUInt16();
                if (twoBytes == 0x8130)
                {
                    reader.ReadByte();
                }
                else if (twoBytes == 0x8230)
                {
                    reader.ReadInt16();
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected value {twoBytes:X4}");
                }

                twoBytes = reader.ReadUInt16();
                if (twoBytes != 0x0102)
                {
                    throw new InvalidOperationException($"Unexpected version {twoBytes:X4}");
                }

                var oneByte = reader.ReadByte();
                if (oneByte != 0x00)
                {
                    throw new InvalidOperationException($"Unexpected value {oneByte:X2}");
                }

                var rsa = RSA.Create();
                var rsaParameters = new RSAParameters
                {
                    Modulus = reader.ReadBytes(GetBytesCountToRead(reader)),
                    Exponent = reader.ReadBytes(GetBytesCountToRead(reader)),
                    D = reader.ReadBytes(GetBytesCountToRead(reader)),
                    P = reader.ReadBytes(GetBytesCountToRead(reader)),
                    Q = reader.ReadBytes(GetBytesCountToRead(reader)),
                    DP = reader.ReadBytes(GetBytesCountToRead(reader)),
                    DQ = reader.ReadBytes(GetBytesCountToRead(reader)),
                    InverseQ = reader.ReadBytes(GetBytesCountToRead(reader))
                };

                rsa.ImportParameters(rsaParameters);

                return rsa;
            }
        }

        private static RSA CreateRsaFromPublicKey(byte[] publicKey)
        {
            using (var stream = new MemoryStream(publicKey))
            using (var reader = new BinaryReader(stream))
            {
                var twoBytes = reader.ReadUInt16();

                // Little endian order
                if (twoBytes == 0x8130)
                {
                    reader.ReadByte();
                }
                else if (twoBytes == 0x8230)
                {
                    reader.ReadInt16();
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected value {twoBytes:X4}");
                }

                var oid = reader.ReadBytes(15);
                if (!oid.SequenceEqual(Pkcs1Oid))
                {
                    var oidString = string.Join(", ", oid.Select(x => $"0x{x:X2}"));

                    throw new InvalidOperationException($"Unexpected oid: [{oidString}]");
                }

                twoBytes = reader.ReadUInt16();
                if (twoBytes == 0x8103)
                {
                    reader.ReadByte();
                }
                else if (twoBytes == 0x8203)
                {
                    reader.ReadInt16();
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected value {twoBytes:X4}");
                }

                var oneByte = reader.ReadByte();
                if (oneByte != 0x00)
                {
                    throw new InvalidOperationException($"Unexpected value {oneByte:X2}");
                }

                twoBytes = reader.ReadUInt16();
                if (twoBytes == 0x8130)
                {
                    reader.ReadByte();
                }
                else if (twoBytes == 0x8230)
                {
                    reader.ReadInt16();
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected value {twoBytes:X4}");
                }

                twoBytes = reader.ReadUInt16();
                byte lowbyte;
                byte highbyte = 0x00;

                if (twoBytes == 0x8102)
                {
                    lowbyte = reader.ReadByte();
                }
                else if (twoBytes == 0x8202)
                {
                    highbyte = reader.ReadByte();
                    lowbyte = reader.ReadByte();
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected value {twoBytes:X4}");
                }

                // Reverse byte order since asn.1 key uses big endian order
                byte[] modulusSizeBytes = {lowbyte, highbyte, 0x00, 0x00};
                var modulusSize = BitConverter.ToInt32(modulusSizeBytes, 0);

                if (reader.PeekChar() == 0x00)
                {
                    // If first byte (highest order) of modulus is zero, don't include it
                    reader.ReadByte();
                    --modulusSize;
                }

                var modulus = reader.ReadBytes(modulusSize);

                oneByte = reader.ReadByte();
                if (oneByte != 0x02)
                {
                    throw new InvalidOperationException($"Unexpected value {oneByte:X2}");
                }

                int exponentSize = reader.ReadByte();
                var exponent = reader.ReadBytes(exponentSize);

                var rsa = RSA.Create();
                var rsaParameters = new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = exponent
                };
                rsa.ImportParameters(rsaParameters);

                return rsa;
            }
        }

        private static int GetBytesCountToRead(BinaryReader reader)
        {
            int count;
            
            var oneByte = reader.ReadByte();
            if (oneByte != 0x02)
            {
                return 0;
            }

            oneByte = reader.ReadByte();

            if (oneByte == 0x81)
            {
                count = reader.ReadByte();
            }
            else if (oneByte == 0x82)
            {
                var highbyte = reader.ReadByte();
                var lowbyte = reader.ReadByte();
                byte[] intBytes = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(intBytes, 0);
            }
            else
            {
                count = oneByte;
            }

            while (reader.ReadByte() == 0x00)
            {
                count -= 1;
            }

            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            return count;
        }
    }
}

