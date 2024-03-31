using AITool;

using Newtonsoft.Json;

using NPushover.Exceptions;
using NPushover.RequestObjects;
using NPushover.ResponseObjects;
using NPushover.Validators;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NPushover
{
    // NOTE: This library is not written or supported by Superblock (the creators of Pushover).

    // TODO: Implement OpenClient API realtime notifications? https://pushover.net/api/client

    /// <summary>
    /// Provides asynchronous methods to interact with the Pushover service.
    /// </summary>
    public class Pushover
    {
        private static readonly string HOMEURL = "https://github.com/RobThree/NPushOver";
        private static readonly AssemblyName ASSEMBLYNAME = typeof(Pushover).Assembly.GetName();
        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly string USERAGENT = string.Format("{0} v{1} ({2})", ASSEMBLYNAME.Name, ASSEMBLYNAME.Version.ToString(2), HOMEURL);

        private HttpClient httpClient = null;

        #region Public 'consts'
        /// <summary>
        /// The default <see cref="Uri"/> used by Pushover.
        /// </summary>
        public static readonly Uri DEFAULTBASEURI = new Uri("https://api.pushover.net/");

        /// <summary>
        /// The default <see cref="Encoding"/> used by Pushover.
        /// </summary>
        public static readonly Encoding DEFAULTENCODING = Encoding.UTF8;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the encoding used for exchanging messages with Pushover.
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Gets the base URL used by Pushover.
        /// </summary>
        public Uri BaseUri { get; private set; }

        /// <summary>
        /// Gets the application token used to identify to Pushover.
        /// </summary>
        public string ApplicationKey { get; private set; }

        /// <summary>
        /// Gets/sets the <see cref="IWebProxy"/> server used by Pushover.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="IValidator&lt;T&gt;"/> used to validate messages before sending.
        /// </summary>
        public IValidator<Message> MessageValidator { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="IValidator&lt;T&gt;"/> used to validate the Application Key.
        /// </summary>
        public IValidator<string> AppKeyValidator { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="IValidator&lt;T&gt;"/> used to validate user or group keys.
        /// </summary>
        public IValidator<string> UserOrGroupKeyValidator { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="IValidator&lt;T&gt;"/> used to validate devicenames.
        /// </summary>
        public IValidator<string> DeviceNameValidator { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="IValidator&lt;T&gt;"/> used to receipts.
        /// </summary>
        public IValidator<string> ReceiptValidator { get; set; }

        /// <summary>
        /// Gets/sets the <see cref="IValidator&lt;T&gt;"/> used to validate e-mail addresses.
        /// </summary>
        public IValidator<string> EmailValidator { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of <see cref="Pushover"/> with the specified applicationkey, the 
        /// <see cref="DEFAULTBASEURI"/> and <see cref="DEFAULTENCODING"/>.
        /// </summary>
        /// <param name="applicationKey">The application key (or token).</param>
        /// <seealso href="https://pushover.net/api">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when applicationKey is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid applicationKey is specified.</exception>
        public Pushover(string applicationKey)
            : this(applicationKey, DEFAULTBASEURI) { }

        /// <summary>
        /// Initializes a new instance of <see cref="Pushover"/> with the specified applicationkey and base URI and 
        /// <see cref="DEFAULTENCODING"/>.
        /// </summary>
        /// <param name="applicationKey">The application key (or token).</param>
        /// <param name="baseUri">The base <see cref="Uri"/> to use. Note that this does not include the API version (e.g. 1).</param>
        /// <seealso href="https://pushover.net/api">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when applicationKey or baseUri are null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid applicationKey is specified.</exception>
        public Pushover(string applicationKey, Uri baseUri)
            : this(applicationKey, baseUri, DEFAULTENCODING) { }

        /// <summary>
        /// Initializes a new instance of <see cref="Pushover"/> with the specified applicationkey, base URI and <see cref="Encoding"/>.
        /// </summary>
        /// <param name="applicationKey">The application key (or token).</param>
        /// <param name="baseUri">The base <see cref="Uri"/> to use. Note that this does not include the API version (e.g. 1).</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use for exchaning data with Pushover.</param>
        /// <seealso href="https://pushover.net/api">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when applicationKey, baseUri or Encoding are null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid applicationKey is specified.</exception>
        public Pushover(string applicationKey, Uri baseUri, Encoding encoding)
            : this(applicationKey, baseUri, encoding, new ApplicationKeyValidator())
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="Pushover"/> with the specified applicationkey, base URI and <see cref="Encoding"/>.
        /// </summary>
        /// <param name="applicationKey">The application key (or token).</param>
        /// <param name="baseUri">The base <see cref="Uri"/> to use. Note that this does not include the API version (e.g. 1).</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use for exchaning data with Pushover.</param>
        /// <param name="applicationKeyValidator">The <see cref="IValidator&lt;T&gt;"/> to use to validate the application key.</param>
        /// <seealso href="https://pushover.net/api">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when applicationKey, baseUri, Encoding or applicationKeyValidator are null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid applicationKey is specified.</exception>
        public Pushover(string applicationKey, Uri baseUri, Encoding encoding, IValidator<string> applicationKeyValidator)
        {
            if (baseUri == null)
                throw new ArgumentNullException("baseUri");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            if (applicationKeyValidator == null)
                throw new ArgumentNullException("applicationKeyValidator");

            applicationKeyValidator.Validate("applicationKey", applicationKey);

            this.AppKeyValidator = applicationKeyValidator;
            this.ApplicationKey = applicationKey;
            this.BaseUri = baseUri;
            this.Encoding = encoding;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sends, asynchronously, the specified <see cref="Message"/> using Pushover to the specified user or group.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> to send.</param>
        /// <param name="userOrGroup">The user or group id to send the message to.</param>
        /// <returns>Returns the <see cref="PushoverUserResponse"/> returned by the server.</returns>
        /// <seealso href="https://pushover.net/api#messages">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when message or user/group arguments are null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid user/group is specified.</exception>
        public async Task<PushoverUserResponse> SendPushoverMessageAsync(Message message, string userOrGroup, ClsImageQueueItem CurImg)
        {
            return await this.SendPushoverMessageAsync(message, userOrGroup, (string[])null, CurImg).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends, asynchronously, the specified <see cref="Message"/> using Pushover to the specified device of the
        /// specified user or group.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> to send.</param>
        /// <param name="userOrGroup">The user or group id to send the message to.</param>
        /// <param name="deviceName">The devicename to send the message to.</param>
        /// <returns>Returns the <see cref="PushoverUserResponse"/> returned by the server.</returns>
        /// <seealso href="https://pushover.net/api#messages">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when message or user/group arguments are null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid user/group is specified.</exception>
        public async Task<PushoverUserResponse> SendPushoverMessageAsync(Message message, string userOrGroup, string deviceName, ClsImageQueueItem CurImg)
        {
            List<string> devices = deviceName.SplitStr(";");

            return await this.SendPushoverMessageAsync(message, userOrGroup, devices.ToArray(), CurImg).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends, asynchronously, the specified <see cref="Message"/> using Pushover to the specified device(s) of the
        /// specified user or group.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> to send.</param>
        /// <param name="userOrGroup">The user or group id to send the message to.</param>
        /// <param name="deviceNames">The devicenames to send the message to.</param>
        /// <returns>Returns the <see cref="PushoverUserResponse"/> returned by the server.</returns>
        /// <seealso href="https://pushover.net/api#messages">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when message or user/group arguments are null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid user/group is specified.</exception>
        public async Task<PushoverUserResponse> SendPushoverMessageAsync(Message message, string userOrGroup, string[] deviceNames, ClsImageQueueItem CurImg)
        {

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            PushoverUserResponse ret = new PushoverUserResponse();

            try
            {
                (this.MessageValidator ?? new DefaultMessageValidator()).Validate("message", message);
                (this.UserOrGroupKeyValidator ?? new UserOrGroupKeyValidator()).Validate("userOrGroup", userOrGroup);
                if (deviceNames != null && deviceNames.Length > 0)
                {
                    foreach (var device in deviceNames)
                    {
                        if (!string.IsNullOrWhiteSpace(device))
                            (this.DeviceNameValidator ?? new DeviceNameValidator()).Validate("device", device);
                    }
                }

                MultipartFormDataContent parameters = new MultipartFormDataContent();

                parameters.AddConditional("token", this.ApplicationKey);
                parameters.AddConditional("user", userOrGroup);
                parameters.AddConditional("message", message.Body);
                parameters.Add("priority", (int)message.Priority);
                parameters.AddConditional("device", deviceNames);
                parameters.AddConditional("title", message.Title);
                parameters.AddConditional("sound", message.Sound);
                parameters.AddConditional("html", message.IsHtmlBody);
                if (message.SupplementaryUrl != null)
                {
                    parameters.Add("url", message.SupplementaryUrl.Uri);
                    parameters.AddConditional("url_title", message.SupplementaryUrl.Title);
                }

                if (message.Priority == Priority.Emergency)
                {
                    parameters.Add("retry", message.RetryOptions.RetryEvery);
                    parameters.Add("expire", message.RetryOptions.RetryPeriod);
                    parameters.Add("callback", message.RetryOptions.CallBackUrl);
                }
                if (message.Timestamp != null)
                    parameters.Add("timestamp", (int)(TimeZoneInfo.ConvertTimeToUtc(message.Timestamp.Value).Subtract(EPOCH).TotalSeconds));

                if (CurImg != null && CurImg.IsValid())
                {
                    StreamContent imageParameter = new StreamContent(CurImg.ToMemStream());
                    imageParameter.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    parameters.Add(imageParameter, "attachment", Path.GetFileName(CurImg.image_path));
                }

                ret = await this.PushoverPost(GetV1APIUriFromBase("messages.json"), parameters);

            }
            catch (Exception ex)
            {

                AITOOL.Log($"Error: {ex.Msg()}");
                ret.Errors = new string[] { ex.Msg() };
            }

            return ret;

        }


        private async Task<PushoverUserResponse> PushoverPost(Uri uri, MultipartFormDataContent parameters)
        {

            using var Trace = new Trace();  //This c# 8.0 using feature will auto dispose when the function is done.

            PushoverUserResponse ret = new PushoverUserResponse();

            if (this.httpClient == null)
            {
                this.httpClient = new HttpClient();
                this.httpClient.Timeout = TimeSpan.FromSeconds(AppSettings.Settings.HTTPClientRemoteTimeoutSeconds);
            }

            // Remove content type that is not in the docs
            //foreach (HttpContent param in parameters)
            //    param.Headers.ContentType = null;

            try
            {
                HttpResponseMessage response = null;
                string json = "";

                response = await httpClient.PostAsync(uri, parameters);

                json = await response.Content.ReadAsStringAsync();
                json = json.CleanString();

                if (response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(json))
                {
                    //var json = this.Encoding.GetString(await wc.UploadValuesTaskAsync(uri, parameters).ConfigureAwait(false));
                    ret = await ParseResponse<PushoverUserResponse>(json, response.Headers);
                }
                else
                {
                    if (response.StatusCode.ToString().Contains("TooLarge"))
                    {
                        ret.Errors = new string[] { $"StatusCode='{response.StatusCode}', Reason='{response.ReasonPhrase}' (Max pushover attachment size is 2.5MB), ResponseText='{json}'" };
                    }
                    else
                    {
                        ret.Errors = new string[] { $"StatusCode='{response.StatusCode}', Reason='{response.ReasonPhrase}', ResponseText='{json}'" };
                    }
                }
            }
            catch (Exception ex)
            {
                AITOOL.Log($"Error: {ex.Msg()}");
                ret.Errors = new string[] { ex.Msg() };
            }

            return ret;

        }


        /// <summary>
        /// Retrieves, asynchronously, a list of available sounds.
        /// </summary>
        /// <seealso href="https://pushover.net/api#sounds">Pushover API documentation</seealso>
        /// <returns>Returns a <see cref="SoundsResponse"/>.</returns>
        public async Task<SoundsResponse> ListSoundsAsync()
        {
            return await this.Get<SoundsResponse>(GetV1APIUriFromBase("sounds.json?token={0}", this.ApplicationKey)).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves, asynchronously, information about a receipt.
        /// </summary>
        /// <param name="receipt">The receipt id to retrieve the information for.</param>
        /// <returns>Returns a <see cref="ReceiptResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api#receipt">Pushover API documentation</seealso>
        /// <seealso cref="Message.Priority"/>
        /// <seealso cref="Priority"/>
        /// <exception cref="ArgumentNullException">Thrown when receipt is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid receipt id is specified.</exception>
        public async Task<ReceiptResponse> GetReceiptAsync(string receipt)
        {
            (this.ReceiptValidator ?? new ReceiptValidator()).Validate("receipt", receipt);
            return await this.Get<ReceiptResponse>(GetV1APIUriFromBase("receipts/{0}.json?token={1}", receipt, this.ApplicationKey)).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels, asynchronously, a receipt.
        /// </summary>
        /// <param name="receipt">The receipt id to cancel.</param>
        /// <returns>Returns a <see cref="PushoverUserResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api#receipt">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when receipt is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when an invalid receipt id is specified.</exception>
        public async Task<PushoverUserResponse> CancelReceiptAsync(string receipt)
        {
            (this.ReceiptValidator ?? new ReceiptValidator()).Validate("receipt", receipt);
            var parameters = new NameValueCollection {
                { "token", this.ApplicationKey }
            };
            return await this.Post<PushoverUserResponse>(GetV1APIUriFromBase("receipts/{0}/cancel.json", receipt), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates, asynchronously, a specified user or group with the Pushover service.
        /// </summary>
        /// <param name="userOrGroup">The user or group id to validate.</param>
        /// <returns>Returns a <see cref="ValidateUserOrGroupResponse"/>.</returns>
        /// <remarks>
        /// Currently, this method throws when the user/group is not known by the Pushover service; this is likely to 
        /// change in the future.
        /// </remarks>
        /// <seealso href="https://pushover.net/api#verification">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when user/group id is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when user/group id is invalid.</exception>
        public async Task<ValidateUserOrGroupResponse> ValidateUserOrGroupAsync(string userOrGroup)
        {
            return await ValidateUserOrGroupAsync(userOrGroup, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates, asynchronously, a specified device for a user or group with the Pushover service.
        /// </summary>
        /// <param name="userOrGroup">The user or group id to validate the device for.</param>
        /// <param name="deviceName">The devicename to validate.</param>
        /// <returns>Returns a <see cref="ValidateUserOrGroupResponse"/>.</returns>
        /// <remarks>
        /// Currently, this method throws when the user/group and/or devicename is not known by the Pushover service;
        /// this is likely to change in the future.
        /// </remarks>
        /// <seealso href="https://pushover.net/api#verification">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when user/group id or the devicename is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when user/group id or the devicename is invalid.</exception>
        public async Task<ValidateUserOrGroupResponse> ValidateUserOrGroupAsync(string userOrGroup, string deviceName)
        {
            (this.UserOrGroupKeyValidator ?? new UserOrGroupKeyValidator()).Validate("userOrGroup", userOrGroup);
            if (deviceName != null)
                (this.DeviceNameValidator ?? new DeviceNameValidator()).Validate("device", deviceName);

            var parameters = new NameValueCollection {
                { "token", this.ApplicationKey },
                { "user", userOrGroup }
            };
            return await this.Post<ValidateUserOrGroupResponse>(GetV1APIUriFromBase("users/validate.json"), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Migrates, asynchronously, a specific subscription to a user/group.
        /// </summary>
        /// <param name="subscription">Subscription code to migrate.</param>
        /// <param name="user">User code to migrate the subscription to.</param>
        /// <returns>Returns a <see cref="MigrateSubscriptionResponse"/>.</returns>
        /// <remarks>Applications that formerly collected Pushover user keys are encouraged to migrate to subscription keys.</remarks>
        /// <seealso href="https://pushover.net/api/subscriptions#migration">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when subscription or user is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when user or devicename are invalid.</exception>
        public async Task<MigrateSubscriptionResponse> MigrateSubscriptionAsync(string subscription, string user)
        {
            return await MigrateSubscriptionAsync(subscription, user, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Migrates, asynchronously, a specific subscription to a user/group and limits it to a specified device.
        /// </summary>
        /// <param name="subscription">Subscription code to migrate.</param>
        /// <param name="user">User code to migrate the subscription to.</param>
        /// <param name="device">The device name that the subscription should be limited to.</param>
        /// <returns>Returns a <see cref="MigrateSubscriptionResponse"/>.</returns>
        /// <remarks>Applications that formerly collected Pushover user keys are encouraged to migrate to subscription keys.</remarks>
        /// <seealso href="https://pushover.net/api/subscriptions#migration">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when subscription or user is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when user or devicename are invalid.</exception>
        public async Task<MigrateSubscriptionResponse> MigrateSubscriptionAsync(string subscription, string user, string device)
        {
            return await MigrateSubscriptionAsync(subscription, user, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Migrates, asynchronously, a specific subscription to a user/group and limits it to a specified device, setting the user's preferred default sound.
        /// </summary>
        /// <param name="subscription">Subscription code to migrate.</param>
        /// <param name="user">User code to migrate the subscription to.</param>
        /// <param name="device">The device name that the subscription should be limited to.</param>
        /// <param name="sound">The user's preferred default sound.</param>
        /// <returns>Returns a <see cref="MigrateSubscriptionResponse"/>.</returns>
        /// <remarks>Applications that formerly collected Pushover user keys are encouraged to migrate to subscription keys.</remarks>
        /// <seealso href="https://pushover.net/api/subscriptions#migration">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when subscription or user is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when user or devicename are invalid.</exception>
        public async Task<MigrateSubscriptionResponse> MigrateSubscriptionAsync(string subscription, string user, string device, string sound)
        {
            if (string.IsNullOrEmpty(subscription))
                throw new ArgumentNullException("subscription");
            (this.UserOrGroupKeyValidator ?? new UserOrGroupKeyValidator()).Validate("user", user);
            if (device != null)
                (this.DeviceNameValidator ?? new DeviceNameValidator()).Validate("device", device);

            var parameters = new NameValueCollection {
                { "token", this.ApplicationKey },
                { "subscription", subscription },
                { "user", user },
            };
            parameters.AddConditional("device_name", device);
            parameters.AddConditional("sound", sound);

            return await this.Post<MigrateSubscriptionResponse>(GetV1APIUriFromBase("subscriptions/migrate.json"), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Assigns, asynchronously, a license to a specific user or e-mail address.
        /// </summary>
        /// <param name="user">The user id (required unless e-mail is specified).</param>
        /// <param name="email">The user's e-mail address (required unless user is specified).</param>
        /// <returns>Returns a <see cref="AssignLicenseResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api/licensing#assign">Pushover API documentation</seealso>
        /// <exception cref="InvalidOperationException">When user and email are both null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid <see cref="OS"/> specified.</exception>
        public async Task<AssignLicenseResponse> AssignLicenseAsync(string user, string email)
        {
            return await AssignLicenseAsync(user, email, OS.Any).ConfigureAwait(false);
        }

        /// <summary>
        /// Assigns, asynchronously, a license to a specific user or e-mail address and specified <see cref="OS"/>.
        /// </summary>
        /// <param name="user">The user id (required unless e-mail is specified).</param>
        /// <param name="email">The user's e-mail address (required unless user is specified).</param>
        /// <param name="os">The <see cref="OS"/> to assign the license to.</param>
        /// <returns>Returns a <see cref="AssignLicenseResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api/licensing#assign">Pushover API documentation</seealso>
        /// <exception cref="InvalidOperationException">When user and email are both null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid <see cref="OS"/> specified.</exception>
        public async Task<AssignLicenseResponse> AssignLicenseAsync(string user, string email, OS os)
        {
            if (user != null)
                (this.UserOrGroupKeyValidator ?? new UserOrGroupKeyValidator()).Validate("user", user);
            if (email != null)
                (this.EmailValidator ?? new EMailValidator()).Validate("email", email);

            if (user == null && email == null)
                throw new InvalidOperationException("User or Email required");

            if (!Enum.IsDefined(typeof(OS), os))
                throw new ArgumentOutOfRangeException("os");

            var parameters = new NameValueCollection {
                { "token", this.ApplicationKey },
            };
            parameters.AddConditional("user", user);
            parameters.AddConditional("email", email);
            parameters.AddConditional("os", os);

            return await this.Post<AssignLicenseResponse>(GetV1APIUriFromBase("licenses/assign.json"), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves, asynchronously, the user's Pushover key and secret.
        /// </summary>
        /// <param name="email">The user's e-mail address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>Returns a <see cref="LoginResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#login">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when e-mail or password is null.</exception>
        /// <exception cref="ArgumentException">Thrown when e-mail is invalid.</exception>
        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            (this.EmailValidator ?? new EMailValidator()).Validate("email", email);
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            var parameters = new NameValueCollection {
                { "email", email },
                { "password", password },
            };

            return await this.Post<LoginResponse>(GetV1APIUriFromBase("users/login.json"), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Registers, asynchronously, a device.
        /// </summary>
        /// <param name="secret">The user's secret.</param>
        /// <param name="deviceName">The short name of the device.</param>
        /// <returns>Returns a <see cref="RegisterDeviceResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#register">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when secret or devicename are null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when devicename is invalid.</exception>
        public async Task<RegisterDeviceResponse> RegisterDeviceAsync(string secret, string deviceName)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");
            (this.DeviceNameValidator ?? new DeviceNameValidator()).Validate("deviceName", deviceName);

            var parameters = new NameValueCollection {
                { "secret", secret },
                { "name", deviceName },
                { "os", "O" }, //This is, currently, the only supported value ("Open Client")
            };

            //TODO: BUG? Report? See https://pushover.net/api/client#register and https://www.reddit.com/r/pushover/comments/3abuy0/api_inconsistencies/
            //When an existing devicename is used, a 400 bad request is returned
            //However, the JSON returned is {"errors":{"name":["has already been taken"]},"status":0,"request":"b3e85163d0bd8fc84565839ffc33bb42"}
            //Where "errors" normally is an array, it is now an object... this is not (very) consistent with the rest of the responses
            //The call, currently, throws a BadRequestException, as it should, however the errorresponse fails to parse because of this inconsistency...
            return await this.Post<RegisterDeviceResponse>(GetV1APIUriFromBase("devices.json"), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves, asynchronously, all existing messages waiting for the device.
        /// </summary>
        /// <param name="secret">The user's secret.</param>
        /// <param name="deviceId">Device id for which to download the messages.</param>
        /// <returns>Returns a <see cref="ListMessagesResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#download">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when secret or device id is null.</exception>
        public async Task<ListMessagesResponse> ListMessagesAsync(string secret, string deviceId)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");

            return await this.Get<ListMessagesResponse>(GetV1APIUriFromBase("messages.json?secret={0}&device_id={1}", secret, deviceId)).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes, asynchronously, all message up to, and including, the specified message id.
        /// </summary>
        /// <param name="secret">The user's secret.</param>
        /// <param name="deviceId">Device id for which to delete the messages.</param>
        /// <param name="upToAndIncludingMessageId">Message id of message up to, and including, to delete.</param>
        /// <returns>Returns a <see cref="PushoverUserResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#delete">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when secret or device id is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when message id is invalid.</exception>
        public async Task<PushoverUserResponse> DeleteMessagesAsync(string secret, string deviceId, int upToAndIncludingMessageId)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");
            if (string.IsNullOrEmpty(deviceId))
                throw new ArgumentNullException("deviceId");
            if (upToAndIncludingMessageId < 0)
                throw new ArgumentOutOfRangeException("upToAndIncludingMessageId");

            var parameters = new NameValueCollection {
                { "secret", secret },
            };
            parameters.Add("message", upToAndIncludingMessageId);

            return await this.Post<PushoverUserResponse>(GetV1APIUriFromBase("devices/{0}/update_highest_message.json", deviceId), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Acknowledges, asynchronously, an emergency-priority message.
        /// </summary>
        /// <param name="secret">The user's secret.</param>
        /// <param name="receipt">The receipt of the message to acknowledge.</param>
        /// <returns>Returns a <see cref="PushoverUserResponse"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#p2">Pushover API documentation</seealso>
        /// <seealso cref="Message.Priority"/>
        /// <seealso cref="Priority"/>
        /// <exception cref="ArgumentNullException">Thrown when secret or receipt is null.</exception>
        /// <exception cref="InvalidKeyException">Thrown when receipt is invalid.</exception>
        public async Task<PushoverUserResponse> AcknowledgeMessageAsync(string secret, string receipt)
        {
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException("secret");
            (this.ReceiptValidator ?? new ReceiptValidator()).Validate("receipt", receipt);

            var parameters = new NameValueCollection {
                { "secret", secret }
            };
            return await this.Post<PushoverUserResponse>(GetV1APIUriFromBase("receipts/{0}/acknowledge.json", receipt), parameters).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads, asynchronously, a specified icon from the Pushover service.
        /// </summary>
        /// <param name="iconName">Name of the icon to download.</param>
        /// <returns>Returns a <see cref="Stream"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#download">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when iconname is null.</exception>
        public async Task<Stream> DownloadIconAsync(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
                throw new ArgumentNullException("iconName");
            return await this.DownloadFileAsync(GetIconUriFromBase("{0}.png", iconName)).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads, asynchronously, a specified sound from the Pushover service in Mp3 format.
        /// </summary>
        /// <param name="soundName">Name of the sound to download.</param>
        /// <returns>Returns a <see cref="Stream"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#download">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when soundname is null.</exception>
        public async Task<Stream> DownloadSoundAsync(string soundName)
        {
            return await DownloadSoundAsync(soundName, AudioFormat.Mp3).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads, asynchronously, a specified sound from the Pushover service in the specified format.
        /// </summary>
        /// <param name="soundName">Name of the sound to download.</param>
        /// <param name="audioFormat"><see cref="AudioFormat"/> to download.</param>
        /// <returns>Returns a <see cref="Stream"/>.</returns>
        /// <seealso href="https://pushover.net/api/client#download">Pushover API documentation</seealso>
        /// <exception cref="ArgumentNullException">Thrown when soundname is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when audioFormat is invalid.</exception>
        public async Task<Stream> DownloadSoundAsync(string soundName, AudioFormat audioFormat)
        {
            if (string.IsNullOrEmpty(soundName))
                throw new ArgumentNullException("soundName");
            if (!Enum.IsDefined(typeof(AudioFormat), audioFormat))
                throw new ArgumentOutOfRangeException("audioFormat");

            return await this.DownloadFileAsync(GetSoundsUriFromBase("{0}.{1}", soundName, audioFormat.ToString().ToLowerInvariant())).ConfigureAwait(false);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Downloads, asynchronously, a specified file.
        /// </summary>
        /// <param name="uri"><see cref="Uri"/> of file to download.</param>
        /// <returns>Returns a <see cref="Stream"/>.</returns>
        private async Task<Stream> DownloadFileAsync(Uri uri)
        {
            using (var wc = this.GetWebClient())
                return await wc.OpenReadTaskAsync(uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a relative <see cref="Uri"/> based on the <see cref="BaseUri"/>.
        /// </summary>
        /// <param name="relative">Relative path from the <see cref="BaseUri"/>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>Returns a relative <see cref="Uri"/> based on the <see cref="BaseUri"/>.</returns>
        private Uri GetUriFromBase(string relative, params object[] args)
        {
            return new Uri(this.BaseUri, this.FormatUri(relative, args));
        }

        /// <summary>
        /// Returns the V1 api <see cref="Uri"/> based on the <see cref="BaseUri"/>.
        /// </summary>
        /// <param name="relative">Relative path from the V1 API <see cref="Uri"/>.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>Returns the V1 api <see cref="Uri"/> based on the <see cref="BaseUri"/>.</returns>
        private Uri GetV1APIUriFromBase(string relative, params object[] args)
        {
            return new Uri(GetUriFromBase("1/"), this.FormatUri(relative, args));
        }

        /// <summary>
        /// Returns an icon <see cref="Uri"/> based on the <see cref="BaseUri"/>.
        /// </summary>
        /// <param name="relative">Relative path for the icon.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>Returns an icon <see cref="Uri"/> based on the <see cref="BaseUri"/>.</returns>
        private Uri GetIconUriFromBase(string relative, params object[] args)
        {
            return new Uri(GetUriFromBase("icons/"), this.FormatUri(relative, args));
        }

        /// <summary>
        /// Returns a sound <see cref="Uri"/> based on the <see cref="BaseUri"/>.
        /// </summary>
        /// <param name="relative">Relative path for the sound.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>Returns a sound <see cref="Uri"/> based on the <see cref="BaseUri"/>.</returns>
        private Uri GetSoundsUriFromBase(string relative, params object[] args)
        {
            return new Uri(GetUriFromBase("sounds/"), this.FormatUri(relative, args));
        }

        /// <summary>
        /// Acts like a regular string.Format. However, it escapes each argument for use in a <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The composite (partial) uri(format); e.g. /foo/{0}/bar?baz={1}.</param>
        /// <param name="args">An array that contains zero or more objects to format.</param>
        /// <returns>Returns the escaped <see cref="Uri"/>.</returns>
        private string FormatUri(string uri, params object[] args)
        {
            return string.Format(uri, args.Select(a => Uri.EscapeDataString(a.ToString())).ToArray());
        }



        /// <summary>
        /// Executes a POST to the given <see cref="Uri"/> passing the specified parameters.
        /// </summary>
        /// <typeparam name="T">The type of response to expext.</typeparam>
        /// <param name="uri">The <see cref="Uri"/> to post to.</param>
        /// <param name="parameters">The values to post.</param>
        /// <returns>Returns the parsed response.</returns>
        private async Task<T> Post<T>(Uri uri, NameValueCollection parameters)
            where T : PushoverResponse
        {
            using (var wc = this.GetWebClient())
            {
                return await ExecuteWebRequest<T>(async () =>
                {
                    var json = this.Encoding.GetString(await wc.UploadValuesTaskAsync(uri, parameters).ConfigureAwait(false));
                    return await ParseResponse<T>(json, wc.ResponseHeaders).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes a GET to the given <see cref="Uri"/>.
        /// </summary>
        /// <typeparam name="T">The type of response to expext.</typeparam>
        /// <param name="uri">The <see cref="Uri"/> to GET.</param>
        /// <returns>Returns the parsed response.</returns>
        private async Task<T> Get<T>(Uri uri)
            where T : PushoverResponse
        {
            using (var wc = this.GetWebClient())
            {
                return await ExecuteWebRequest<T>(async () =>
                {
                    var json = await wc.DownloadStringTaskAsync(uri).ConfigureAwait(false);
                    return await ParseResponse<T>(json, wc.ResponseHeaders).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes a specified function asynchronously assuming it returns a <see cref="PushoverResponse"/> and
        /// handles any web exceptions and other problems as best as possible (or re-throws).
        /// </summary>
        private async Task<T> ExecuteWebRequest<T>(Func<Task<T>> func)
            where T : PushoverResponse
        {
            try
            {
                return await func.Invoke().ConfigureAwait(false);
            }
            catch (WebException wex)
            {
                var response = wex.Response as HttpWebResponse;
                if (response != null)
                {
                    //Try parse any json response... IF any
                    var errorresponse = ParseErrorResponse(response);
                    if (errorresponse != null)
                    {
                        switch ((int)response.StatusCode)
                        {
                            case 400:   //Bad request
                                throw new BadRequestException(errorresponse, wex);
                            case 429:   //Rate limited
                                throw new RateLimitExceededException(errorresponse);
                        }
                    }
                    else
                    {
                        throw new ResponseException(string.Format("Unable to parse error response. Status was: {0} {1}", response.StatusCode, response.StatusDescription));
                    }
                }
                throw;
            }
            catch (PushoverException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PushoverException("Error retrieving response", ex);
            }
        }

        /// <summary>
        /// When Pushover returns an error it is returned in a (sort of...) structured format; this method tries to
        /// parse the result and extract possible information from it.
        /// </summary>
        private PushoverUserResponse ParseErrorResponse(HttpWebResponse response)
        {
            PushoverUserResponse errorresponse = null;
            try
            {
                using (var s = response.GetResponseStream())
                using (var r = new StreamReader(s, this.Encoding))
                {
                    var json = r.ReadToEnd();
                    errorresponse = JsonConvert.DeserializeObject<PushoverUserResponse>(json);
                }

                errorresponse.RateLimitInfo = ParseRateLimitInfo(response.Headers);
            }
            catch
            {
                //NOP
            }
            return errorresponse;
        }

        private static RateLimitInfo ParseRateLimitInfo(HttpResponseHeaders headers)
        {
            int limit, remaining, reset;

            if (int.TryParse(headers.GetValues("X-Limit-App-Limit").FirstOrDefault(), out limit)
                && int.TryParse(headers.GetValues("X-Limit-App-Remaining").FirstOrDefault(), out remaining)
                && int.TryParse(headers.GetValues("X-Limit-App-Reset").FirstOrDefault(), out reset))
            {
                return new RateLimitInfo(limit, remaining, EPOCH.AddSeconds(reset));
            }
            return null;
        }

        /// <summary>
        /// Parses, possible, rate-limiting information from a Pushover response if any.
        /// </summary>
        private static RateLimitInfo ParseRateLimitInfo(WebHeaderCollection headers)
        {
            int limit, remaining, reset;

            if (int.TryParse(headers["X-Limit-App-Limit"], out limit)
                && int.TryParse(headers["X-Limit-App-Remaining"], out remaining)
                && int.TryParse(headers["X-Limit-App-Reset"], out reset))
            {
                return new RateLimitInfo(limit, remaining, EPOCH.AddSeconds(reset));
            }
            return null;
        }


        private static async Task<T> ParseResponse<T>(string json, HttpResponseHeaders headers)
            where T : PushoverResponse
        {
            T result;
            try
            {
                result = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PushoverException("Error parsing response", ex);
            }

            result.RateLimitInfo = ParseRateLimitInfo(headers);
            if (!result.IsOk)
                throw new ResponseException("API returned one or more errors", result);

            return result;
        }

        /// <summary>
        /// Parses a Pushover JSON response asynchronously.
        /// </summary>
        private static async Task<T> ParseResponse<T>(string json, WebHeaderCollection headers)
            where T : PushoverResponse
        {
            T result;
            try
            {
                result = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new PushoverException("Error parsing response", ex);
            }

            result.RateLimitInfo = ParseRateLimitInfo(headers);
            if (!result.IsOk)
                throw new ResponseException("API returned one or more errors", result);

            return result;
        }

        /// <summary>
        /// Creates and returns a <see cref="WebClient"/> to be used when communicating with Pushover's service.
        /// </summary>
        private WebClient GetWebClient()
        {
            var wc = new WebClient();
            wc.Proxy = this.Proxy;
            wc.Headers.Add(HttpRequestHeader.UserAgent, USERAGENT);
            wc.Encoding = this.Encoding;
            return wc;
        }
        #endregion
    }
}
