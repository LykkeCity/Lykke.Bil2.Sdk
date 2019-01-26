using Lykke.Blockchains.Integrations.Contract.Common;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class EncryptedStringTests
    {
        #region Constants

        private const string PublicKey1024 = "Vj75CuZgqYqhewfDfF9KQEdejjqbnoDiRjuXUnwCo2jkLo8AJpqBF6jFovufKrvwqUaubTRrAwr3wBBHtFVWhxhrxwFMoeB3mrBXnreVkfRdL1L9NUpyn4qDTB1Hwm3kBjmnhdVm2ZxmZ696FKj6yeBMnPB7Lkoa4XxKg6a9TwPCfgUbNV9b8dCPXm1YdYMdFK9Hf8sestxpp6FphTxbrjnicpeiU";

        private const string PrivateKey1024 = "hLLPYvqqGdBtPBMGWp98NbAHtDHuoj4vnrQqNPuMBMZpRFCX2kVtbC9HgrMQthM6NzEDjQw6c88PfiRCqzegb2KpTnBN1hqLyQKfT8FuGVDGRBJEYFZbgUbbb5e5PZZVnRwabyy4Rup8c5Yb5fWJ1Lw73yUtcxvmx52ymwm8m9j3NtVRTkcb5NsXGbMdanUpa9MdAMHReFDDh1qWx8N6zTb4st4WbaTuTamhcTEueuPKosaum5TLv2udcUDj2JtaZYUhDJ6EvotsHScwLQnwcW4ZkEydSCJgqefYp4n7qgNpzNKU2ffsDmPuQDfVBLNiFkqv99TMYsvrYbemcLLRoEUiqvGTaGZBpHErd5quYq1G9187nbySGy3aJCMVqVWzApuYKewUQDRND1fx6kybQpuDy6CjBp4i5sdAub4eFNHjRwVVUUsYQjjhy8PQpES5AWszv2DgMyrqEaAGYxr3akK6HmbYyBquWriAdYjp9v8NW29yhpSBXwyGpCT5j54KA2F5yaxv6MfUiucGHxYdgNFdkhSZYRB3FgZJQzTAy6utSZTapjcBmu25nzQcdaevnSW4rMd9kuhMGJL6PtyG6niHn48bsmwxSukGZsMvfRb8UDf3ZnGnNFHXChADzywM7ghZBtohBRgbnAyduUQ7GbLv6vhuyXV7qxAkvAwuz4mipe8MbTem6cjhKJrV1Sa4Ya6pUnWR7BhqNjWEKuVbCt8D9Et2no6u8hXxyxV6kD5tKEJ932PVHYBmnLQV9jCfS2RRv2BhBjhA6bUp8zCSuW2CyVYPCgmqQCa4EahA6iiHhymdaPQ8YGChbH8san";

        private const string PublicKey2048 = "2TuPVgMCHJy5atawrsADEzjP7MCVbyyCA89UW6Wvjp9HrAdA2FGxJJzeBKrbpcjMk6N3zbzxLUoGuLX2f1PaAuXfZtxPsTraJQLECYqr6EkwMrmrhfnPRPt5MkB5MmA2wdVkKaFUsJ2tPx4S1pqVhv913DL6ZYXzNR8RbEyoMASeX5eEq178tpjFNXT159xqrq72gwqy4JATHLm3FS3CqV2LndyH7uNgFhMwMXeVWXFezNrbub7YfBujU97kUDSqp7fxijbvsneh9w1GSe8gzw9Du7AJDCURSA8wp3LC1o4pQETkk5qc1L1eQ8LPf1WjW3mmkmAAAWBW3GbZ4UTpfuUQYno2bHS51YVAkdbuQyut7Ygot8hHxehUd1tbsNzRd86sXMbD35Tjf1y62L";

        private const string PrivateKey2048 = "7LsR8WfToH83zUcc6qDW2XTTPmi5R5BB6sBfW74DCo4PA8FkpbgMMtBxuywKuZZGHn6bMCnX7ETgnsLB1RGK2XqnpcU4USuZaqBW7x5D5ZhNwRA6ACq3xbnT2usqfZ8wgUUfRmsdzxNSCWK96gidZW36Lt43EGLBNfvWz6ruPZ9kDBQFdyWMQUaDk8VAu8bMroQ9aVqXuMmaJRH447y527jb7RE6jxcJFEoEwn5xvuxtsUsaVy9JbgeSH7GrrQhzxp6srW1fGJMw3cmscn5JgHw6jCSsQKh9DAtDWBik2rhFKAPj926nFun7wKZwW8mBvmwAwx1T6rgJnfGsp9safTjkevjEW41h3zW9diF7Sbd38XYFyRsJbMKZFk18Yd4nmkvnHgsyjuqVKuMMBYNyY1zX5SbvZKN4vafr5XrAb5ahXsByGjF1zWG3cfBs6HKTViqVvRiHrMM9xo3LZkawqSBfER2vnQ5JNX3m7qC3wA85JT928aJB9H4RY5k2YPjK3bxuEvmN76wsU8S1HjQGTohVbHQWnKKzUkC3ycC88S1ho97ffgYERRBV2NG3eBbbMLXC3PmWVqvBxRnbHjMzztLwEuJNpoJSVPnFBit2oiKntXLBNAp1s42YUXJdRWqxH3XSvV3D2DmpzRe7z3pmmW2pvyo7kU5MnCx8EAHQ236vfmNN4BGGwjUZPW5siyHrUv7mvujHi4CTQLkqNun6CwK8Y412931Fz91AZf86ijAoHmnFvsFpr4oyi1nudBxfXgNghV8ioS8E8ao4kY31rygnABeLYe3FDbWGVkdtppRKPGAePAdje8eyeEHHigym1TZYy1B6YqTnhZjrYC7ozYDkovNav6CrgtEiahi5Nhp1D5tNX8iNs1n1wgHrhNr24rQpt71akCBXLFHFEzah8L1k2SU9yjcU6BxJCiZxoWUVLCCiv8RFwgkFWGkKVtmDAWNLf8Mg63ZqgnWcJCpBWCiSuoUKxKndjeaYHCG1EQSw3jFThiBNM6zQ6p7QFVWkdCYe3VbZpeZ4AdSmio9bLfBEbJtRmQtcJp4BZiEbVsjPP8isFhZ5xL2mUDQU1THANuFMkmnTo3S353uaZ3rjN55NA5FAxYWq7Pd7NeNPhsZHyBLsGgdJ1gfvwRscARcVn84V5jm89pLmUGSWxcFAtsgkrGRLVfbGmxmrQtcbE9V7f592obboxHPRTytLiUQeVxmDHoTW5hDNbAVm7WMCihHNz1ZLEizZ5XXAkxi8Dr9J2SaLfRHmLxE2T4ViFGpuAzFaX5s12cq4LgA26sPS3h9xneXy2cgeV5F22gyFeUwGXUfJPysYnqCEYcyA1C4EP9KiyfdhfNnZYr4q8uUnBidDCpDTNNi3gBYtKJ1tybiUDRu5XW1GeDrWjGPQpSytP35d3UM6gdCQPdjcFBKgZFnHQAxBSScsZXvcPd6it4nrst34ur7FzmJ1w9zPZEc4KZEM3BrGZJmLfnYo6PkM1VVT8HApY3PY2U7uLehas7YNfogto6iMeNVbH3czWoxrvfcKYZqUs4t8MfaS5FYXkUfE1qdLzX3wj2XFi2jpXJGMfjnRuPZpcrYChASjfCDwEVBHNLQopiR7HCrLtwWNjwjfqZcN";

        private const string LongPlainString = "He share of first to worse. Weddings and any opinions suitable smallest nay. My he houses or months settle remove ladies appear. Engrossed suffering supposing he recommend do eagerness. Commanded no of depending extremity recommend attention tolerably. Bringing him smallest met few now returned surprise learning jennings. Objection delivered eagerness he exquisite at do in. Warmly up he nearer mr merely me. By so delight of showing neither believe he present. Deal sigh up in shew away when. Pursuit express no or prepare replied. Wholly formed old latter future but way she. Day her likewise smallest expenses judgment building man carriage gay. Considered introduced themselves mr to discretion at. Means among saw hopes for. Death mirth in oh learn he equal on. Demesne far hearted suppose venture excited see had has. Dependent on so extremely delivered by. Yet ﻿no jokes worse her why. Bed one supposing breakfast day fulfilled off depending questions. Whatever boy her exertion his extended. Ecstatic followed handsome drawings entirely mrs one yet outweigh. Of acceptance insipidity remarkably is invitation. Nor hence hoped her after other known defer his. For county now sister engage had season better had waited. Occasional mrs interested far expression acceptance. Day either mrs talent pulled men rather regret admire but. Life ye sake it shed. Five lady he cold in meet up. Service get met adapted matters offence for. Principles man any insipidity age you simplicity understood. Do offering pleasure no ecstatic whatever on mr directly. Sense child do state to defer mr of forty. Become latter but nor abroad wisdom waited. Was delivered gentleman acuteness but daughters. In as of whole as match asked. Pleasure exertion put add entrance distance drawings. In equally matters showing greatly it as. Want name any wise are able park when. Saw vicinity judgment remember finished men throwing. Two exquisite objection delighted deficient yet its contained. Cordial because are account evident its subject but eat. Can properly followed learning prepared you doubtful yet him. Over many our good lady feet ask that. Expenses own moderate day fat trifling stronger sir domestic feelings. Itself at be answer always exeter up do. Though or my plenty uneasy do. Friendship so considered remarkably be to sentiments. Offered mention greater fifteen one promise because nor. Why denoting speaking fat indulged saw dwelling raillery. Smallest directly families surprise honoured am an. Speaking replying mistress him numerous she returned feelings may day. Evening way luckily son exposed get general greatly. Zealously prevailed be arranging do. Set arranging too dejection september happiness. Understood instrument or do connection no appearance do invitation. Dried quick round it or order. Add past see west felt did any. Say out noise you taste merry plate you share. My resolve arrived is we chamber be removal. New had happen unable uneasy. Drawings can followed improved out sociable not. Earnestly so do instantly pretended. See general few civilly amiable pleased account carried. Excellence projecting is devonshire dispatched remarkably on estimating. Side in so life past. Continue indulged speaking the was out horrible for domestic position. Seeing rather her you not esteem men settle genius excuse. Deal say over you age from. Comparison new ham melancholy son themselves. Yourself off its pleasant ecstatic now law. Ye their mirth seems of songs. Prospect out bed contempt separate. Her inquietude our shy yet sentiments collecting. Cottage fat beloved himself arrived old. Grave widow hours among him ﻿no you led. Power had these met least nor young. Yet match drift wrong his our. In as name to here them deny wise this. As rapid woody my he me which. Men but they fail shew just wish next put. Led all visitor musical calling nor her. Within coming figure sex things are. Pretended concluded did repulsive education smallness yet yet described. Had country man his pressed shewing. No gate dare rose he. Eyes year if miss he as upon.";

        private const string EncryptedText = "3NDKENv6MoxaxNZbeHsTHBSNjRkP9PqDEULNJNAqtd25P4H4BA8FfVAefMpHb7Bx4FY5CoN9CQfMoHqoEMF9zQCW8a5ryJEmQo512ZF6soJHZ8cxcfV92dahRzEz4DA1ZkQPZ4Eikdrg66ePGCmE6DhBLV6HbCVcfa8JQiGV69w1bVTF437hqacGqPBjP3syPxThGWRGsL3sQZm7Rudii23pvSzRSyXVye4YePPi6Sa7J6NxmknpavQgrjagh82g7DA79aiBFDhPBdVqQZXHyj7soJQ9DpyShyhEuK1cngigZswL2D4DaK63aM7DeEs38YXZ8C4";

        #endregion

        [Test]
        [TestCase(PublicKey1024, PrivateKey1024, "")]
        [TestCase(PublicKey1024, PrivateKey1024, "1")]
        [TestCase(PublicKey1024, PrivateKey1024, LongPlainString)]
        [TestCase(PublicKey2048, PrivateKey2048, LongPlainString, TestName = "2048Key")]
        public void Can_encrypt_and_decrypt(string publicKey, string privateKey, string stringToEncrypt)
        {
            var value = EncryptedString.Encrypt(Base58String.Create(publicKey), stringToEncrypt);
            
            Assert.AreNotEqual(stringToEncrypt, value.EncryptedValue);

            var decryptedString = value.DecryptToString(Base58String.Create(privateKey));

            Assert.AreEqual(stringToEncrypt, decryptedString);
        }

        [Test]
        [TestCase(PrivateKey1024, EncryptedText, ExpectedResult = "1")]
        public string Can_create_from_encrypted_text(string privateKey, string encryptedString)
        {
            var value = EncryptedString.Create(Base58String.Create(encryptedString));

            var decryptedString = value.DecryptToString(Base58String.Create(privateKey));

            return decryptedString;
        }
    }
}
