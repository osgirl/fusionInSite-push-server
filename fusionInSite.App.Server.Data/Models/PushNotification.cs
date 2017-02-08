﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionInsite.App.Server.Data.Models
{
    public class UserPushNotification
    {
        public UserPushNotification(PushNotification pushNotification, string token)
        {
            PushNotification = pushNotification;
            Token = token;
        }

        public PushNotification PushNotification { get; set; }

        public string Token { get; set; }

        public UserPushNotification WithUserToken(string token)
        {
            Token = token;
            return this;
        }
    }

    public class PushNotification
    {
        public int ShipmentKey { get; set; }
        public int InventoryKey { get; set; }
        public int? ProtocolId { get; set; }
        public PushNotificationType PushNotificationType { get; set; }
        public int? StatusId { get; set; }
        public string Message { get; set; }
    }

    public class UserMessage : IEquatable<UserMessage>
    {
        public List<string> Token { get; set; }

        public List<PushNotification> PushNotifications { get; set; }

        public string Message { get; set; }

        public UserMessage()
        {
            PushNotifications = new List<PushNotification>();
        }
        

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (var foo in PushNotifications.Select(p => p.ShipmentKey).Union(PushNotifications.Select(p => p.InventoryKey)))
                {
                    hash = hash * 31 + foo.GetHashCode();
                }
                return hash;
            }
        }

        public bool Equals(UserMessage other) // Compare the ShipmentKeys and the InventoryKeys
        {
            if (other == null) return false; 

            var keys = PushNotifications.Select(p => p.ShipmentKey).Concat(PushNotifications.Select(p => p.InventoryKey)).ToList();
            var otherkeys = other.PushNotifications.Select(p => p.ShipmentKey).Concat(other.PushNotifications.Select(p => p.InventoryKey)).ToList();

            var comparer1 = keys.Except(otherkeys);
            var comparer2 = otherkeys.Except(keys);
             
            return Message == other.Message && !comparer1.Any() && !comparer2.Any();
        }
    }

}
