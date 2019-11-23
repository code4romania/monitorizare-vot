using System;
using VoteMonitor.Api.Core.Services;

namespace VoteMonitor.Api.Observer.Utils
{
    public class RandomObserverBuilder
    {
        private readonly IHashService _hashService;
        private readonly Entities.Observer _observer;

        private RandomObserverBuilder(IHashService hashService)
        {
            _hashService = hashService;
            _observer = new Entities.Observer();
        }

        public static RandomObserverBuilder Instance(IHashService hashService) => new RandomObserverBuilder(hashService);

        public Entities.Observer Build(int idNgo)
        {
            SetFromTeam(false);
            SetIdNgo(idNgo);
            SetPhone(RandomNumberGenerator.GenerateWithPadding(10, "07"));
            SetPin(_hashService.GetHash(RandomNumberGenerator.Generate(6)));
            SetMobileDeviceId(null);
            SetDeviceRegisterDate(null);
            SetName(string.Empty);

            return _observer;
        }

        public void SetDeviceRegisterDate(DateTime? dateTime)
        {
            _observer.DeviceRegisterDate = dateTime;
        }

        public void SetFromTeam(bool fromTeam)
        {
            _observer.FromTeam = fromTeam;
        }

        public void SetIdNgo(int idNgo)
        {
            _observer.IdNgo = idNgo;
        }

        public void SetMobileDeviceId(string mobileDeviceId)
        {
            _observer.MobileDeviceId = mobileDeviceId;
        }

        public void SetPhone(string phone)
        {
            _observer.Phone = phone;
        }

        public void SetPin(string pin)
        {
            _observer.Pin = pin;
        }

        public void SetName(string name)
        {
            _observer.Name = name;
        }
    }

    public class RandomNumberGenerator
    {
        public static string Generate(int digits)
        {
            Random random = new Random();
            string number = "";
            for (int i = 1; i < digits + 1; i++)
            {
                number += random.Next(0, 9).ToString();
            }
            return number;
        }

        public static string GenerateWithPadding(int digits, string prefix)
        {
            Random random = new Random();
            string number = prefix;
            for (int i = 1 + prefix.Length; i < digits + 1; i++)
            {
                number += random.Next(0, 9).ToString();
            }
            return number;
        }
    }
}