<%@ Page Title=""  Language="C#" MasterPageFile="~/RspPSMaster.Master"
	AutoEventWireup="True" EnableEventValidation="false" CodeBehind="EHS_InjuryIllnessForm.aspx.cs" ClientIDMode="AutoID"
	Inherits="SQM.Website.EHS_InjuryIllnessForm" ValidateRequest="false" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="~/Include/Ucl_AdminTabs.ascx" TagName="AdminTabs" TagPrefix="Ucl" %> 
<%@ Register Src="~/Include/Ucl_INCFORM_InjuryIllness.ascx" TagName="InjuryIllness" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_RadGauge.ascx" TagName="RadGauge" TagPrefix="Ucl" %>
<%@ Register Src="~/Include/Ucl_Export.ascx" TagName="Export" TagPrefix="Ucl" %>


<asp:Content ID="HeaderContent" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">

		function StandardConfirm(sender, args) {

			// Some pages will have no validators, so skip
			if (typeof Page_ClientValidate === "function") {
				var validated = Page_ClientValidate('Val');

				if (!validated)
					alert("Please fill out all required fields.");
			}
		}
		function DeleteConfirm(button, args) {
			args.set_cancel(!confirm("Delete incident - are you sure?  Incidents cannot be undeleted."));
		}

		function DeleteConfirmItem(button, args) {
			args.set_cancel(!confirm("Delete item - are you sure?  Items cannot be undeleted."));
		}

	</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Body" runat="server">

	<div class="pageWrapper">
		<div class="container-fluid tabActiveTableBg">
			<Ucl:InjuryIllness ID="uclIncidentForm" runat="server" vislble="false"/>
			<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
			</telerik:RadAjaxManager>
		</div>
	</div>

</asp:Content>

