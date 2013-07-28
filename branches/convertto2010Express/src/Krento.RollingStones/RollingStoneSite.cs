using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class RollingStoneWikipedia : RollingStoneFolder
    {
        public RollingStoneWikipedia(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebWiki.png";
                TranslationId = SR.Keys.Wikipedia;
                TargetDescription = null;
                Path = "http://www.wikipedia.org";
                Args = "";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneGoogle : RollingStoneFolder
    {
        public RollingStoneGoogle(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebGoogle.png";
                TranslationId = SR.Keys.Google;
                TargetDescription = null;
                Path = "http://www.google.com";
                Args = "";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneGMail : RollingStoneFolder
    {
        public RollingStoneGMail(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebGmail.png";
                TranslationId = SR.Keys.GMail;
                TargetDescription = null;
                Path = "http://www.gmail.com";
                Args = "";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneTwitter : RollingStoneFolder
    {
        public RollingStoneTwitter(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebTwitter.png";
                TranslationId = SR.Keys.Twitter;
                TargetDescription = null;
                Path = "http://twitter.com";
                Args = "";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneBlogger : RollingStoneFolder
    {
        public RollingStoneBlogger(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebBlogger.png";
                TranslationId = SR.Keys.Blogger;
                TargetDescription = null;
                Path = "http://www.blogger.com";
                Args = "";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneFaceBook : RollingStoneFolder
    {
        public RollingStoneFaceBook(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebFacebook.png";
                TranslationId = SR.Keys.Facebook;
                TargetDescription = null;
                Args = "";
                Path = "http://www.facebook.com";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneYouTube : RollingStoneFolder
    {
        public RollingStoneYouTube(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebYoutube.png";
                TranslationId = SR.Keys.YouTube;
                TargetDescription = null;
                Args = "";
                Path = "http://www.youtube.com";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneWordpress : RollingStoneFolder
    {
        public RollingStoneWordpress(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebWordpress.png";
                TranslationId = SR.Keys.Wordpress;
                TargetDescription = null;
                Args = "";
                Path = "http://wordpress.com";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneYahoo : RollingStoneFolder
    {
        public RollingStoneYahoo(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebYahoo.png";
                TranslationId = SR.Keys.Yahoo;
                TargetDescription = null;
                Args = "";
                Path = "http://www.yahoo.com";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneReddit : RollingStoneFolder
    {
        public RollingStoneReddit(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebReggit.png";
                TranslationId = SR.Keys.Reddit;
                TargetDescription = null;
                Args = "";
                Path = "http://www.reddit.com";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneFlickr : RollingStoneFolder
    {
        public RollingStoneFlickr(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebFlickr.png";
                TranslationId = SR.Keys.Flickr;
                TargetDescription = null;
                Args = "";
                Path = "http://www.flickr.com";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneDelicious : RollingStoneFolder
    {
        public RollingStoneDelicious(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "WebDelicious.png";
                TranslationId = SR.Keys.Delicious;
                TargetDescription = null;
                Args = "";
                Path = "http://www.delicious.com";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

}
