   M S B A M L   `   `   `  ����   
RootFinder    RootFinder.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Root Finder��$��567.911��$��947.529��.��]� .+�o� $��25��$	��Top=�$	��Top=�.@�m� $��_File��.@�m� -   $��Open _XML table��m� -    m�Name$ 	btnReport��$��Save _report��{�.   � ToolTip$/ )Save report on last root finding process.��m� -   $��_Exit��m� $��_Edit��.@�m� -   $ menEditXMLTable��$��_Edit XML data table��{�.  m� -   $��_Convert function to XML��m� $��_Chart��.@�m� -   $ menShowChart��$��_Show chart��{�.  m� -   $ menChartCfg��$��_Config��{��� *	IsEnabled+ �� ElementName$ menShowChart��m� $��_Properties��.@�m� -   $��Search interval��{��� *	IsEnabled+$ btnFindRoots��m� -	   $��Accuracy��m� $��_Help��.@�m� -
   $��
_Show help��m� -   $��_About��� $	��Top=� � ColumnDefinitions �� $
��263* ��� $
��733* �.+��� $��5��
���   ��� -    ��Name$ txtFx��$��1��$��25��$��118,76,70,0q� �� TextWrapping$
 Wrap=�$	��Top=�$2 ,Here must be your function, dependant on 'x'��$��Times New Roman)�$��Italic$�$��15(��� $
��F(x)��$��1��$
��Left=�$��	69,76,0,0q�$	��Top=�$��25��$��30���� $
��G(x)��$��1��$
��Left=�$��
68,156,0,0q�$	��Top=�$��24���� -   $��
Create New��$��1��$
��Left=�$��118,156,0,0q�$	��Top=�$��75���� -   $��Load from XML��$��1��$
��Left=�$��220,156,0,0q�$	��Top=�$��96���� -    ��Name$ btnSaveChangesToXML��$��Save changes to XML��$��1��$
��Left=�$��340,156,0,0q�$	��Top=�$	��126��{�.  �� -   $ btnFindRoots��$��
Find roots��$
��Left=�$��	24,0,0,10q�$	��182��$��35��$��Bottom=�9� -    9�Name$ 
expSummary��$��Summary��$��	0,10,19,0q�$	��Top=�$	��440��{�.  .��� .+��� $��Search interval:��$
��Left=�$��8,10,0,0q�$	��Top=��� -   	 ��Name$	 lblSrcInterval��$��[lblSrcInterval]��$��123,10,19,0q�$	��Top=��� $
��Eps.��$
��Left=�$��8,36,0,0q�$	��Top=��� -   $	 lblEps��$��[lblEps]��$��123,36,56,0q�$	��Top=��� $��Interpolation error:��$
��Left=�$��8,62,0,0q�$	��Top=��� -   $	 lblInterpError��$��[lblInterpError]��$��123,62,10,0q�$	��Top=��� $��Number of roots:��$
��Left=�$��8,93,0,0q�$	��Top=��� -   $	 lblRoots��$��
[lblRoots]��$
��Left=�$��
123,93,0,0q�$	��Top=�$��94���� -   $��Show all roots��$
��Left=�$��123,124,0,0q�$	��Top=�$��94���� $#��Power of Lagrange polynomial:��$
��Left=�$��
10,172,0,0q�$	��Top=��� -   $	 lblLagPower��$��[lblLagPower]��$
��Left=�$��123,198,0,0q�$	��Top=�$��85���� $��Lagrange polynomial:��$
��Left=�$��
10,230,0,0q�$	��Top=��� -   $
��Show��$
��Left=�$��139,234,0,0q�$	��Top=�$��75��&   System.Windows.Controls.DataGrid  -   
  Name$
 	pointGrid��   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * $��1��$
��Left=�$��68,185,0,38q�$	��554��$��Stretch=�"   VerticalScrollBarVisibility$
 Auto=�   
IsReadOnly .    CanUserDeleteRows . ���� �� -      shakePictoBtn#��[  $��1��$��Right=�$��	0,46,70,0q�$	��Top=����� $��Save-as-icon.png��$
��Fill=��� -   #��[  $��1��$
��Left=�$��
567,46,0,0q�$	��Top=����� $��open-file-icon.png��$
��Fill=�