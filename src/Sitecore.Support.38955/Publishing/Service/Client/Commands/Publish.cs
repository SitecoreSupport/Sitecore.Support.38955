using Sitecore.Data.Items;
using Sitecore.Framework.Conditions;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Publishing.Service.Client.Commands
{
  public class Publish : Sitecore.Publishing.Service.Client.Commands.Publish
  {
    protected void Run(ClientPipelineArgs args)
    {
      Condition.Requires<ClientPipelineArgs>(args, "args").IsNotNull<ClientPipelineArgs>();
      Condition.Requires<NameValueCollection>(args.Parameters, "context.Parameters").IsNotNull<NameValueCollection>();
      string itemPath = args.Parameters["id"];
      string name = args.Parameters["language"];
      string value = args.Parameters["version"];
      Item item = Context.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value)];
      if (item == null)
      {
        SheerResponse.Alert("Item not found.", new string[0]);
        return;
      }
      if (!this.IsWorkflowPublishable(args, item))
      {
        return;
      }
      SheerResponse.Broadcast(SheerResponse.ShowModalDialog(new ModalDialogOptions(string.Format("{0}?id={1}&sourceDatabase={2}", "/sitecore/shell/sitecore/client/Applications/Publishing/PublishDialog", item.ID, item.Database.Name))
      {
        Width = "650",
        Height = "750",
        ForceDialogSize = true
      }), "Shell");
    }
  }
}