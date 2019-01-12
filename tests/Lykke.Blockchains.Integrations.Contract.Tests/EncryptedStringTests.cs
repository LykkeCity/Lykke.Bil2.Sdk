using Lykke.Blockchains.Integrations.Contract.Common;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class EncryptedStringTests
    {
        #region Constants

        private const string PublicKey512 = 
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCYK+LcJ2ws2l4OXA3X5LlmK" +
            "fVIrbcGADjYwAt/SmR/vDUHbWyoL28PswcVXEqw4fO3sq1Ck5c3k677ruHcpm" +
            "mhaDYc3G8vROYwgobsD+FgZqjyBAsxB3+tJcKlUtDzjOaaq0FxSm2cOEgQK7D" +
            "0ldmdqceybDfigDRod756xJdUFwIDAQAB";

        private const string PrivateKey512 =
            "MIICXAIBAAKBgQCYK+LcJ2ws2l4OXA3X5LlmKfVIrbcGADjYwAt/SmR/vDUHb" +
            "WyoL28PswcVXEqw4fO3sq1Ck5c3k677ruHcpmmhaDYc3G8vROYwgobsD+FgZq" +
            "jyBAsxB3+tJcKlUtDzjOaaq0FxSm2cOEgQK7D0ldmdqceybDfigDRod756xJd" +
            "UFwIDAQABAoGAFMbJLq3jQyx9cxB2g2ejOKO57bZqKtOU72MpLrQFjLsxslXq" +
            "Y/w1+brD2NLFD+mJ0ScAKPrlxpzPY2W5SNsfyMbpvPXMlxZTQVbd1Xg8oITM2" +
            "M5R71T+7S4oyzdzEsOkRkXcboFsVQvRTAod9I74fonFNgsEyH584+OK7md7P0" +
            "ECQQDxNM4/b+4iVv3xSMp5j0EQhAiMWaIuD9cizanWQJNWNTS3pjCfoysq2vH" +
            "zAGgldhU6VptaCvutJTt8WSl1U8FPAkEAoYEjmPKGvrOZRdnffyTlu5zMTzfc" +
            "ss5qLdCqdPILG5kyZJhERrCfSBD/rtkZDttOsXrv7SF4wUgZK/Ofvui+uQJBA" +
            "IJhWc7+kMktHq0q/I9CuRfVVs2OsdSWKWMdql0uoLWrouhWQ9g2meHbYYdJxA" +
            "Hj10umfujoIOyRwJrRk1BhSo8CQEvtBCkxSzt3/4ShKrsBQ6dxzXMole7Rr4U" +
            "eZiRYbfRpjxFPrDl3a0pcA3fVxDwByfsSCp12cOic1oidHeqITLECQFc405EI" +
            "oK7ksU5rB7+SOIP9NZQfaBzWbwk2xRHC4uUtz7x3hZJQmIOECWXQ9DsldkOrY" +
            "XGvQWVq+AC1jGRk9oc=";

        private const string PublicKey2048 =
            "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnN1sEiGojZm3D+/Vk" +
            "+64WzZA/qwTzKazZ/FWaLuI1PogL7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Ga" +
            "zdx+zP56TS4Q8eMptM+oTjPEFJDpgYOXWGchZh16g2vWPChCqaGP6NfmrpAiI" +
            "cRqd8dm7yZA+XhkfwiJBLKPEeYKeCVvllycIeyXJRrQB0HlP659wBX6cDavJO" +
            "TQi4Xe6VnsMQk1h0qItuTz6bQ7Zo9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvv" +
            "kPpgF1UH6GOBT1F0zL7fzwThDV9cSkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3" +
            "y5G4utyYuYo/9xaEn92QIDAQAB";

        private const string PrivateKey2048 =
            "MIIEpAIBAAKCAQEAnN1sEiGojZm3D+/Vk+64WzZA/qwTzKazZ/FWaLuI1PogL" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "1GBwsXYP5jjUTz48osWO3SIRd2QJXikimErqJmokDMUP5lHb4u6tecujXnXzy" +
            "RCIw==";

        private const string LongString =
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M" +
            "7fjXbEz3cM5Ezlp5a5ctYySNX/sPvA+Gazdx+zP56TS4Q8eMptM+oTjPEFJDp" +
            "gYOXWGchZh16g2vWPChCqaGP6NfmrpAiIcRqd8dm7yZA+XhkfwiJBLKPEeYKe" +
            "CVvllycIeyXJRrQB0HlP659wBX6cDavJOTQi4Xe6VnsMQk1h0qItuTz6bQ7Zo" +
            "9yQXpo50cH2+NW/VVVGuQmH7Z2hOKZsvvkPpgF1UH6GOBT1F0zL7fzwThDV9c" +
            "SkT/1kaxxrYjn+BYu09B/4ShVrpE7TvP3y5G4utyYuYo/9xaEn92QIDAQABAo" +
            "IBAB5ef93Cxm8jo11igWbygJ6oEk0741lrrLAi+SetpwAUrMaQQ2SQOgplEVe" +
            "3ddkRuMEtReE1svc8a8lsvkNGhbnDH1CTLLV/e5fEwnmX6hz5NTdqmWzdCHRI" +
            "DND9z1rnJI5SOfKPoCzZjb/gDSigsizNF/jXVKobxXFXvRnZVFKiCFxCGTFzo" +
            "w7dflyuWAiZvThbZjZKTLVH7VHw2vNGq0Wwe1zk25H0V71HgNOOtUhQy2JmX8" +
            "li9rNv3G72CoAi4ukiSh0vWzNmmWHX6nqzUxVDFXvIr+BNRYDUmxf3b4p152E" +
            "rpVVPu6t7jcIyo60DXtkOlLaNovTBpTLWQ//MOAECgYEA1pltakjzctHmwo1R" +
            "pEO+6vuVWE97a/fkwJTxGeGPEw75hTjDI79nwxSIEQK98AfQgFN8zXsAikZbr" +
            "wRMoVCcS/0VmwheMHQlNT2NRhfQV4ocG5WAF5yaczU5G70Ncc2HQHON+7+KVy" +
            "OHUQWz/TD7ObOGUr6ab6BTikfbSHWPqTECgYEAuyCf0DF3rWnHGz9mpE087f7" +
            "Z2Ty0e67K0NjYa9ZZykLsT7D9EFDj4ntiCKOzxvQq/mEVjpgo6mea9ozNfnGQ" +
            "OXUNbLDdKQMk0Ol6XiZJdv1sh3tde+J/e5V6izafcWkp/kxGSzAJIgjMHjHX3" +
            "01wMErNxNjcET12zlmmcXYc9SkCgYEAhivxM+WeiGhiHbubktdkc6iObGNtaE" +
            "jaNeopgCP9f036hefCYgRXDKo2lGQhewR3wPPx8Mr5wHpS8m7+tlEfF4eJzZ+" +
            "CvEqxxMLSJEPZWBIalsh3CMw56NQa+5PM5x04dfyJhh/wj6DABtFJAoFwYVg0" +
            "IGHZTvdvo+JfYMNX6dECgYARAZIElbpOpEJYE5b8b5iN+I9L0tyEMQp1mH9yr" +
            "XRTkWlwhKE7Dl7nglcSee4osqAFpkUTRHjTdL78kSdLyn5U+nJR4nD2/TYnVl" +
            "CspcYfbkCiv29rhtYXISqV5EzxPaF3Xu80fhUWZr5GMDlyLdN4UBQLQ7ocRv2" +
            "5GySpQxa2oQKBgQCgoC8zVavaW42d54P1/Q6CBrzYa0UvHYmczL2UdDXxbTE6" +
            "J1PqwE9Ix2bkjiifU3wEe+07Q4kOAaLTsTgMDXhdCGCOmG0gUc6v6oBWav/8M";

        private const string EncryptedText =
            "eyJlbmNyeXB0ZWRBZXNLZXlzIjoiZllJeHJCcGhjNGMvVEVlbUJwbVV5Mis5N" +
            "Dd3THVTWGQ5Yk9lZHVrbThPWDNOVVhZMVJVZUpJTnJRM1REM1Y1UTRheWsvNn" +
            "F6VzNoVE52S0g5U1Bad3JIcUhqWXJ5MVFnQ1pNL3F4RVBIWFp4eDNiUzE5TXU" +
            "3cVVnSFJ2MUQ0eG9DcW1UK3lxakNpTTlnWDVsZFFocnc2UEVuaUdodVlBMWcr" +
            "a3lrS2xFcFpvPSIsImVuY3J5cHRlZEJvZHkiOiJwUytKenk4L2djdFVHcmlyM" +
            "WpmaTVBPT0ifQ==";

        #endregion

        [Test]
        [TestCase(PublicKey512, PrivateKey512, "")]
        [TestCase(PublicKey512, PrivateKey512, "1")]
        [TestCase(PublicKey512, PrivateKey512, LongString)]
        [TestCase(PublicKey2048, PrivateKey2048, LongString)]
        public void Can_encrypt_and_decrypt(string publicKey, string privateKey, string stringToEncrypt)
        {
            var value = EncryptedString.Encrypt(Base64String.Create(publicKey), stringToEncrypt);
            
            Assert.AreNotEqual(stringToEncrypt, value.EncryptedValue);

            var decryptedString = value.DecryptToString(Base64String.Create(privateKey));

            Assert.AreEqual(stringToEncrypt, decryptedString);
        }

        [Test]
        [TestCase(PrivateKey512, EncryptedText, ExpectedResult = "1")]
        public string Can_create_from_encrypted_text(string privateKey, string encryptedString)
        {
            var value = EncryptedString.Create(Base64String.Create(encryptedString));

            var decryptedString = value.DecryptToString(Base64String.Create(privateKey));

            return decryptedString;
        }
    }
}
