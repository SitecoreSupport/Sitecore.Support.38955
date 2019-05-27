using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Framework.Conditions;
using Sitecore.Web.UI.Sheer;
using System.Collections.Specialized;

namespace Sitecore.Support.Publishing.Service.Client.Commands
{
  [Serializable]
  public class PublishSite : Sitecore.Publishing.Service.Client.Commands.PublishSite
  {
    public override void Execute(CommandContext context)
    {
      Condition.Requires<CommandContext>(context, "context").IsNotNull<CommandContext>();
      Condition.Requires<NameValueCollection>(context.Parameters, "context.Parameters").IsNotNull<NameValueCollection>();

      Context.ClientPage.Start(this, "Run", new NameValueCollection());
    }

    protected virtual void Run(ClientPipelineArgs args)
    {   
      if (!SheerResponse.CheckModified(new CheckModifiedParameters
      {
        ResumePreviousPipeline = true
      }))
      {
        return;
      }

      SheerResponse.Broadcast(SheerResponse.ShowModalDialog(new ModalDialogOptions(string.Format("{0}?sourceDatabase={1}", "/sitecore/shell/sitecore/client/Applications/Publishing/PublishDialog", Context.ContentDatabase.Name))
      {
        Width = "650",
        Height = "750",
        ForceDialogSize = true
      }), "Shell");
    }
  }
}