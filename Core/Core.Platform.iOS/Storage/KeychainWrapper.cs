﻿using System;
using Foundation;
using Security;

namespace AeroGear.Mobile.Core.Storage
{
	public class KeychainWrapper
    {
		private readonly string KeychainService;
		private static readonly bool DefaultSynchronize = false;

		public KeychainWrapper(string keychainService)
		{
			KeychainService = keychainService;
        }

        /// <summary>
        /// Save the specified value using the specified key as an identifier.
		/// This key can later be used to retrieve and remove the value.
        /// </summary>
		/// <returns><c>true</c> if the save operation succeeded. Otherwise <c>false</c>.</returns>
        /// <param name="key">Identifier.</param>
        /// <param name="value">Value to store.</param>
        public bool Save(string key, string value)
		{
			Console.WriteLine("=== SAVE ===");
			bool val = SetPassword(KeychainService, key, value, SecAccessible.Always, DefaultSynchronize) == SecStatusCode.Success;
			Console.WriteLine(val);
			return val;
		}

        /// <summary>
        /// Load the value associated with the specified key.
        /// </summary>
        /// <returns>The string value associated with the specified key.</returns>
        /// <param name="key">Key that identifies the stored value.</param>
        public string Load(string key)
		{
			return GetPassword(KeychainService, key, DefaultSynchronize);
		}

        /// <summary>
        /// Remove the value associated with the specified key.
        /// </summary>
		/// <returns><c>true</c> if the remove operation succeeded. Otherwise <c>false</c>.</returns>
        /// <param name="key">Identifier.</param>
        public bool Remove(string key)
		{
			return DeletePassword(KeychainService, key, DefaultSynchronize) == SecStatusCode.Success;
		}

        /// <summary>
        /// Retrieves a password from the Keychain.
        /// </summary>
		/// <returns>The stored password if it exists. Else <c>null</c>.</returns>
        /// <param name="service">Service.</param>
        /// <param name="username">Username associated with the password.</param>
        /// <param name="synchronizable">Whether the item is synchronizable through iCloud.</param>
		private string GetPassword(string service, string username, bool synchronizable)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service", "Service cannot be null");
			}
			if (username == null)
			{
				throw new ArgumentNullException("username", "Username cannot be null");
			}

			SecRecord secRecord = new SecRecord(SecKind.GenericPassword)
			{
				Service = service,
				Account = username,
				Synchronizable = synchronizable
			};
			SecStatusCode secStatusCode;
			SecRecord queryRecord = SecKeyChain.QueryAsRecord(secRecord, out secStatusCode);

			Console.WriteLine("=== SUPER DUPER ===");
			Console.WriteLine(queryRecord == null);

			if (secStatusCode == SecStatusCode.Success && queryRecord != null && queryRecord.Generic != null)
			{
				return NSString.FromData(queryRecord.Generic, NSStringEncoding.UTF8);
			}
			return null;
		}

        /// <summary>
        /// Deletes the password from the Keychain.
        /// </summary>
		/// <returns><see cref="SecRecord"/>.</returns>
        /// <param name="service">Service.</param>
        /// <param name="username">Username associated with the password.</param>
		/// <param name="synchronizable">Whether the item is synchronizable through iCloud.</param>
		private SecStatusCode DeletePassword(string service, string username, bool synchronizable)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service", "Service cannot be null");
			}
			if (username == null)
			{
				throw new ArgumentNullException("username", "Username cannot be null");
			}

			SecRecord secRecord = new SecRecord(SecKind.GenericPassword)
			{
				Service = service,
				Account = username,
				Synchronizable = synchronizable
			};
			return SecKeyChain.Remove(secRecord);
		}
        
        /// <summary>
        /// Set the password in the Keychain.
        /// </summary>
		/// <returns><see cref="SecStatusCode"/>.</returns>
        /// <param name="service">Service.</param>
        /// <param name="username">Username associated with the password.</param>
        /// <param name="password">Password to store.</param>
		/// <param name="accessible"><see cref="SecAccessible"/>.</param>
		/// <param name="synchronizable">Whether the item is synchronizable through iCloud.</param>
		private SecStatusCode SetPassword(string service, string username, string password, SecAccessible accessible, bool synchronizable)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service", "Service cannot be null");
            }
            if (username == null)
            {
                throw new ArgumentNullException("username", "Username cannot be null");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password", "Password cannot be null");
            }

			SecRecord secRecord = new SecRecord(SecKind.GenericPassword)
			{
				Service = service,
				Label = service,
				Account = username,
				Generic = NSData.FromString(password, NSStringEncoding.UTF8),
				Accessible = accessible,
				Synchronizable = synchronizable
			};
            return SecKeyChain.Add(secRecord);
        }
    }
}
