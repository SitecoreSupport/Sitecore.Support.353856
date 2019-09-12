namespace Sitecore.Support.Shell.Framework.Commands.Shell
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Data.Templates;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Text;
    using Sitecore.Web;
    using System;

    [Serializable]
    public class EditImage : Command
    {
        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                item = this.GetItemInCurrentContentLanguage(item);
                UrlString urlString = new UrlString();
                urlString.Append("sc_content", item.Database.Name);
                urlString.Append("id", item.ID.ToString());
                urlString.Append("la", item.Language.ToString());
                urlString.Append("vs", item.Version.ToString());
                if (!string.IsNullOrEmpty(context.Parameters["frameName"]))
                {
                    urlString.Add("pfn", context.Parameters["frameName"]);
                }
                Sitecore.Shell.Framework.Windows.RunApplication("Media/Imager", urlString.ToString());
            }
        }

        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The <see cref="T:Sitecore.Shell.Framework.Commands.CommandState" />.
        /// </returns>
        public override CommandState QueryState(CommandContext context)
        {
            if (WebUtil.GetQueryString("mo") == "preview")
            {
                return CommandState.Hidden;
            }
            if (context.Items.Length != 1)
            {
                return CommandState.Disabled;
            }
            Item item = context.Items[0];   // Command context is always carrying item in default language only.
            item = this.GetItemInCurrentContentLanguage(item);

            if (item == null)
            {
                return CommandState.Disabled;
            }
            if (Command.IsLockedByOther(item))
            {
                return CommandState.Disabled;
            }
            if (item.TemplateID != TemplateIDs.VersionedImage && item.TemplateID != TemplateIDs.UnversionedImage)
            {
                Template template = TemplateManager.GetTemplate(item.TemplateID, item.Database);
                Assert.IsNotNull(template, typeof(Template));
                if (!template.DescendsFrom(TemplateIDs.UnversionedImage) && !template.DescendsFrom(TemplateIDs.VersionedImage))
                {
                    return CommandState.Disabled;
                }
            }
            if (item.Appearance.ReadOnly)
            {
                return CommandState.Disabled;
            }
            if (!item.Access.CanRead())
            {
                return CommandState.Disabled;
            }
            if (!item.Access.CanWrite())
            {
                return CommandState.Disabled;
            }
            if (!item.Access.CanWriteLanguage())
            {
                return CommandState.Disabled;
            }
            if (new MediaItem(item).MimeType.ToLower() == "image/svg+xml")
            {
                return CommandState.Disabled;
            }
            return base.QueryState(context);
        }

        private Item GetItemInCurrentContentLanguage(Item item)
        {
            if (Sitecore.Security.Accounts.User.Current.Profile.ContentLanguage.Length != 0 && (item.Language.ToString() != Sitecore.Security.Accounts.User.Current.Profile.ContentLanguage))
            {
                return Sitecore.Context.ContentDatabase.GetItem(item.ID, LanguageManager.GetLanguage(Sitecore.Security.Accounts.User.Current.Profile.ContentLanguage));
            }
            else
            {
                return item;
            }
        }
    }
}