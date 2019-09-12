namespace Sitecore.Support.Shell.Framework.Commands.WebDAV
{
    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Data.Templates;
    using Sitecore.Diagnostics;
    using Sitecore.SecurityModel;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Shell.Framework.Commands.WebDAV;
    using System;

    [Serializable]
    public class CompositeEdit : Sitecore.Shell.Framework.Commands.WebDAV.CompositeEdit
    {
        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The state of the command.</returns>
        public override CommandState QueryState(CommandContext context)
        {
            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }
            Item item = context.Items[0];
            return SitecoreSupportGetEditingCommand(item)?.QueryState(context) ?? CommandState.Hidden;
        }

        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                SitecoreSupportGetEditingCommand(item)?.Execute(context);
            }
        }

        protected Command SitecoreSupportGetEditingCommand(Item item)
        {
            Assert.IsNotNull(item, "item");
            ID iD = ID.Parse("{962B53C4-F93B-4DF9-9821-415C867B8903}");
            ID iD2 = ID.Parse("{611933AC-CE0C-4DDC-9683-F830232DB150}");
            if (item.TemplateID != iD2 && item.TemplateID != iD)
            {
                Template template = TemplateManager.GetTemplate(item.TemplateID, item.Database);
                Assert.IsNotNull(template, typeof(Template));
                if (!template.DescendsFrom(iD) && !template.DescendsFrom(iD2))
                {
                    return null;
                }
            }
            string ribbon = item.Appearance.Ribbon;
            if (string.IsNullOrEmpty(ribbon))
            {
                return null;
            }
            if (Context.Database == null)
            {
                return null;
            }
            using (new SecurityDisabler())
            {
                Item item2 = Context.Database.GetItem(ribbon);
                if (item2 == null)
                {
                    return null;
                }
                if (string.Compare(item2.Name, "Viewable Media", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return new EditMedia();
                }
                if (string.Compare(item2.Name, "Media", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return new EditMedia();
                }
                if (string.Compare(item2.Name, "Playable Media", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return new EditMedia();
                }
                if (string.Compare(item2.Name, "Images", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    Edit edit = new Edit();
                    return edit.GetEditingCommand();
                }
                return null;
            }
        }
    }
}
    