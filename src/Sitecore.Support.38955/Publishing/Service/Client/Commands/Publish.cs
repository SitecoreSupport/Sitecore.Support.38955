using Sitecore.Data.Items;
using Sitecore.Framework.Conditions;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Publishing.Service.Client.Commands
{
  [Serializable]
  public class Publish : Sitecore.Publishing.Service.Client.Commands.Publish
  {
    protected void Run(ClientPipelineArgs args)
    {
      Condition.Requires<ClientPipelineArgs>(args, "args").IsNotNull<ClientPipelineArgs>();
      Condition.Requires<NameValueCollection>(args.Parameters, "context.Parameters").IsNotNull<NameValueCollection>();
      string itemPath = args.Parameters["id"];
      string name = args.Parameters["language"];
      string value = args.Parameters["version"];

      // 38955 Check if item was modified before publishing
      if (!SheerResponse.CheckModified(new CheckModifiedParameters
      {
        ResumePreviousPipeline = true
      }))
      {
        return;
      }

      Item item = Context.ContentDatabase.Items[itemPath, Language.Parse(name), Sitecore.Data.Version.Parse(value)];
      if (item == null)
      {
        SheerResponse.Alert("Item not found.", new string[0]);
        return;
      }
      if (!(bool)typeof(Sitecore.Publishing.Service.Client.Commands.Publish)
        .GetMethod("IsWorkflowPublishable", BindingFlags.Instance | BindingFlags.NonPublic)
        .Invoke(this, new object[] { args, item }))
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