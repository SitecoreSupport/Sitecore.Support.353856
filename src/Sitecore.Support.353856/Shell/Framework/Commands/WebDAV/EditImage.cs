namespace Sitecore.Support.Shell.Framework.Commands.WebDAV
{
    using Sitecore.Shell.Framework.Commands;
    using System;

    [Serializable]
    public class EditImage : Sitecore.Support.Shell.Framework.Commands.Shell.EditImage
    {
        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The state of the command.</returns>
        public override CommandState QueryState(CommandContext context)
        {
            if (!IsAdvancedClient())
            {
                return CommandState.Hidden;
            }
            return base.QueryState(context);
        }
    }
}