<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">


<xsl:template match="export">
 <html>
 <head>
 </head>
 <body>

	<table cellpadding="2" cellspacing="0" border="0" style='page-break-after:always;margin-left:30px;min-width:400px;' bgcolor='#FFFFFF'>
	<colgroup>
		<col width="50"/>
		<col width="150"/>
		<col width="150"/>
		<col width="250"/>
	</colgroup>

	<tr>
	<td></td>
	<td align='center' style='font-weight:bold;margin-left:30px;font-family:Tahoma; min-height:30px;font-size:12px;color:black'>
		<xsl:value-of select="tournament" />
	</td>
	</tr>	
	<tr>
	<td></td>
	<td align='center' style='font-weight:bold;margin-left:30px;font-family:Tahoma; min-height:30px;font-size:12px;color:black'>
		<xsl:value-of select="title" />
	</td>
	</tr>				
			
	<xsl:for-each select="board">
			
			<tr>
			<td align='center' style='font-weight:bold;font-family:Tahoma; font-size:9px;color:black'>
				<xsl:value-of select="nr" />
			</td>
			<td align='center' style='font-weight:bold;font-family:Tahoma; font-size:9px;color:black'>
				<xsl:value-of select="w" />
			</td>
			<td align='center' style='font-weight:bold;font-family:Tahoma; font-size:9px;color:black'>
				<xsl:value-of select="b" />
			</td>
			<td align='center' style='font-weight:bold;font-family:Tahoma; font-size:9px;color:black'>
				<xsl:value-of select="res" />
			</td>
			<td>		
			</td>
			</tr>
	</xsl:for-each>	

	</table>

	
 </body>
 </html>
</xsl:template>

</xsl:stylesheet>
