//-----------------------------------------------------------------------
// <copyright file="PhotoService.cs" company="Sondre Bjellås">
// This software is licensed as Microsoft Public License (Ms-PL).
// </copyright>
//-----------------------------------------------------------------------

using FlickrNet;

namespace FlickrDownloader
{
    /// <summary>
    /// Represents an instance of any type of photo service that the UI can act upon.
    /// </summary>
    public class PhotoService : IPhotoService
    {
        /// <summary>
        /// Returns a shared instanec of the Photo Service. Useful for application that only has
        /// a single activate use.
        /// </summary>
        public static PhotoService Instance { get; private set; }

        static PhotoService()
        {
            Instance = new PhotoService();
        }

        public Flickr Service { get; set; }
        public SearchOptions SearchOptions { get; set; }
        public Photo[] SelectedPhotos { get; set; }
        public PhotoSearchOptions Options { get; set; }

        public PhotoService()
        {
            Service = FlickrFactory.GetInstance();
            Reset();
        }

        public void Reset()
        {
            SearchOptions = new SearchOptions();
            Options = new PhotoSearchOptions();

            //AddLicenses(Options);
        }

        public void ResetAuthentication()
        {
            UserId = string.Empty;
            Username = string.Empty;
            IsAuthenticated = false;
            Service.AuthToken = string.Empty;

            Properties.Settings.Default.FlickrAuthorizationKey = "";
            Properties.Settings.Default.Save();
        }

        public void PersistToken(string token)
        {
            Service.AuthToken = token;

            FlickrDownloadr.Properties.Settings.Default.FlickrAuthorizationKey = token;
            FlickrDownloadr.Properties.Settings.Default.Save();
        }

        public Auth Authorize()
        {
            var auth = Service.AuthCheckToken(Properties.Settings.Default.FlickrAuthorizationKey);
            //labelStatus.Text = string.Format("You are authenticated as {0}.", auth.User.Username);

            UserId = auth.User.UserId;
            Username = auth.User.Username;
            IsAuthenticated = true;

            Service.AuthToken = auth.Token;

            return auth;
        }

        public bool HasToken
        {
            get { return !string.IsNullOrEmpty(Properties.Settings.Default.FlickrAuthorizationKey); }
        }

        public bool IsAuthenticated { get; set; }

        public string Username { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// Finds the user id based upon username or email. Returns null if not found.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FoundUser FindUserId(string userName)
        {
            FoundUser user = null;

            try
            {
                user = Service.PeopleFindByUsername(userName);
            }
            catch (FlickrNet.FlickrApiException ex1)
            {
                int code = ex1.Code;

                if (code == 1)
                {
                    try
                    {
                        user = Service.PeopleFindByEmail(userName);
                    }
                    catch (FlickrNet.FlickrApiException ex2)
                    {
                        // Swallow and return null
                    }
                }
            }

            return user;
        }

        private PhotoSearchOptions CreateOptions()
        {
            var options = new PhotoSearchOptions();
            options.SafeSearch = SafetyLevel.None;
            return options;
        }

        //private PhotoSearchOptions GetOptionsBasedOnCurrentSearch()
        //{
        //    PhotoSearchOptions options = CreateOptions();

        //    switch (SearchOptions.Type)
        //    {
        //        case SearchType.Tag:
        //            options.Tags = SearchOptions.Text;
        //            break;
        //        case SearchType.Search:
        //            options.Text = SearchOptions.Text;
        //            break;
        //        case SearchType.Group:
        //            options.GroupId = SearchOptions.Text;
        //            break;
        //        default:
        //            options = new PhotoSearchOptions(SearchOptions.UserId);
        //            break;
        //    }

        //    return options;
        //}

        public void AddLicenses(PhotoSearchOptions options, bool creativeCommonsOnly)
        {
            //if (Program.Debug)
            //{
            //    options.AddLicense(0);
            //}

            //options.Licenses.AddLicense(1);
            //options.AddLicense(2);
            //options.AddLicense(3);
            //options.AddLicense(4);
            //options.AddLicense(5);
            //options.AddLicense(6);

            options.Licenses.Clear();

            if (!creativeCommonsOnly)
            {
                options.Licenses.Add(LicenseType.AllRightsReserved);
            }

            options.Licenses.Add(LicenseType.AttributionCC);
            options.Licenses.Add(LicenseType.AttributionNoDerivsCC);
            options.Licenses.Add(LicenseType.AttributionNonCommercialCC);
            options.Licenses.Add(LicenseType.AttributionNonCommercialNoDerivsCC);
            options.Licenses.Add(LicenseType.AttributionNonCommercialShareAlikeCC);
            options.Licenses.Add(LicenseType.AttributionShareAlikeCC);
            options.Licenses.Add(LicenseType.NoKnownCopyrightRestrictions);
        }

        /// <summary>
        /// Returns all photos based on the current search terms, also photos licensed with "All Rights Reserved".
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public Photos Preview(int page, int perPage, bool creativeCommonsOnly)
        {
            AddLicenses(Options, creativeCommonsOnly);

            Options.PerPage = perPage;
            Options.Page = page;
            Options.Extras = PhotoSearchExtras.All;
            return Service.PhotosSearch(Options);
        }

        /// <summary>
        /// Returns only photos that users are allowed to download and is licensed under Creative Commons.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public Photos Search(int page, int perPage, bool creativeCommonsOnly)
        {
            AddLicenses(Options, creativeCommonsOnly);

            Options.PerPage = perPage;
            Options.Page = page;
            Options.Extras = PhotoSearchExtras.All;
            return Service.PhotosSearch(Options);
        }

        public Photos Search(int page)
        {
            return Search(page, 20);
        }

        public Photos Search(int page, int perPage)
        {
            return Search(page, perPage, false);
        }

    }

    public class Photos
    {
    }

    public interface IPhotoService
    {
    }
}
