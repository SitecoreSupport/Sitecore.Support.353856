namespace Sitecore.Support.Shell.Framework.Commands.WebDAV
{
    using Sitecore.Configuration;
    using Sitecore.Shell;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Shell.Framework.Commands.WebDAV;
    using System;

    [Serializable]
    public class Edit : Sitecore.Shell.Framework.Commands.WebDAV.Edit
    {
        public override CommandState QueryState(CommandContext context)
        {
            Command editingCommand = this.GetEditingCommand();
            if (editingCommand == null)
            {
                return CommandState.Hidden;
            }
            CommandState commandState = editingCommand.QueryState(context);
            if (commandState == CommandState.Disabled || commandState == CommandState.Hidden)
            {
                return CommandState.Disabled;
            }
            return commandState;
        }

        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            this.GetEditingCommand()?.Execute(context);
        }

        /// <summary>
        /// Gets the editing command.
        /// </summary>
        /// <returns>The editing command.</returns>
        public override Command GetEditingCommand()
        {
            if (UserOptions.WebDAV.UseLocalEditor)
            {
                if (WebDAVConfiguration.IsWebDAVEnabled(true))
                {
                    return new EditMedia();
                }
                return new EditImage();
            }
            if (!IsAdvancedClient() && WebDAVConfiguration.IsWebDAVEnabled(true))
            {
                return new EditMedia();
            }
            return new EditImage();
        }
    }
}