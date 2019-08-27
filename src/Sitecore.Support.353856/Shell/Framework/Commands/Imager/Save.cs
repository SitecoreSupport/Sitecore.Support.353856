using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.IO;
using Sitecore.Resources.Media;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI;
using Sitecore.Web.UI.Sheer;
using System;
using System.Drawing;
using System.IO;

/// <summary>
/// Represents the Save command.
/// </summary>
[Serializable]
public class Save : Command
{
    /// <summary>
    /// Executes the command in the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <contract>
    ///   <requires name="context" condition="not null" />
    /// </contract>
    public override void Execute(CommandContext context)
    {
        Assert.ArgumentNotNull(context, "context");
        if (context.Items.Length == 1)
        {
            Item item = context.Items[0];
            string filename = MainUtil.MapPath(context.Parameters["WorkFile"]);
            using (Image image = Image.FromFile(filename))
            {
                ImageMedia imageMedia = MediaManager.GetMedia(MediaUri.Parse(item)) as ImageMedia;
                if (imageMedia != null)
                {
                    imageMedia.SetImage(image);
                    Context.ClientPage.Modified = false;
                    if (!string.IsNullOrEmpty(context.Parameters["ParentFrameName"]))
                    {
                        ScriptInvokationBuilder scriptInvokationBuilder = new ScriptInvokationBuilder("scForm", "postMessage");
                        scriptInvokationBuilder.AddString("item:updated(id={0})", item.ID.ToString());
                        scriptInvokationBuilder.AddString(context.Parameters["ParentFrameName"]);
                        scriptInvokationBuilder.AddString("Shell");
                        scriptInvokationBuilder.Add(false);
                        SheerResponse.Eval(scriptInvokationBuilder.ToString());
                    }
                    if (context.Parameters["alert"] != "0")
                    {
                        SheerResponse.Alert("The image has been saved.");
                    }
                }
                else
                {
                    SheerResponse.Alert("The image could not be saved.");
                }
            }
            return;
        }
        string path = MainUtil.MapPath(context.Parameters["File"]);
        string path2 = MainUtil.MapPath(context.Parameters["WorkFile"]);
        if (FileUtil.Exists(path2))
        {
            FileInfo fileInfo = new FileInfo(FileUtil.MapPath(path2));
            fileInfo.CopyTo(FileUtil.MapPath(path), true);
            Context.ClientPage.Modified = false;
            SheerResponse.Alert("The image has been saved.");
        }
        else
        {
            SheerResponse.Alert("The image could not be saved.");
        }
    }

    /// <summary>
    /// Queries the state of the command.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The state of the command.</returns>
    /// <contract>
    ///   <requires name="context" condition="not null" />
    /// </contract>
    public override CommandState QueryState(CommandContext context)
    {
        Assert.ArgumentNotNull(context, "context");
        if (context.Parameters["HasFile"] != "1")
        {
            return CommandState.Disabled;
        }
        if (context.Items.Length != 1)
        {
            return CommandState.Disabled;
        }
        Item item = context.Items[0];
        if (item == null)
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
        using (new LanguageSwitcher(Sitecore.Context.ContentLanguage))
        {
            if (!item.Access.CanWriteLanguage())
            {
                return CommandState.Disabled;
            }
        }
        return base.QueryState(context);
    }
}