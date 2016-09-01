<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VideoHandler.aspx.cs" Inherits="SQM.Website.Shared.VideoHandler" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title></title>
</head>
<body>
	<div style="max-width: 200px;">
		<video controls="controls" autoplay="true" runat="server" id="videoControl" class="video">
			<source runat="server" id="srcControl" />
			Your browser does not support the video tag
		</video>
	</div>
</body>
</html>

