   M S B A M L   `   `   `  ���� !  Pointel.Interactions.Outbound>    8Pointel.Interactions.Outbound.UserControls.UCMyCampaigns   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6my-http://schemas.microsoft.com/wpf/2008/toolkit  :d2http://schemas.microsoft.com/expression/blend/2008    � 	Resources   �� MergedDictionaries �� Source��  �� $V P/Pointel.Interactions.Outbound;component/Resources/StylesResourceDictionary.xaml?�%    .���� ���#ADAAAD$��1q�$��5q�$
��Auto��$
��Auto��
�������._��  � RowDefinitions �� $��60 ��� $��* �.+��� -   $��btnGetReord��$��45��$��55��$��55��$��2q�   NormalImageButton#��[  $	��Top=�$��Stretch=�$
��Left=�{��� *IsEnableGetRecord+.����  �� Orientation$ Vertical=�$��Center=�.+��� $C��=/Pointel.Interactions.Outbound;component/Images/GetRecord.png��$��40��$��40���� $��Center=� �� TextAlignment$ Center=�$��
Get Record��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q�{��� *IsEnableRelease+ � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��ReleaseHeading��$��
Get Record��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��ReleaseContent��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$&�� Get Record from Outbound Contact��)  #Microsoft.Windows.Controls.DataGrid  -   $��DGMyCampaigns��   RowHeaderWidth$ 0Q����#ADAAAD$��2q�
�������$
��Left=�$	��Top=�$
��Left=�	   AutoGenerateColumns	 .  
   CanUserAddRows
 .     HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   CanUserDeleteRows .  $��1��   HeadersVisibility$ Column=�   SelectionMode$ Single=����� *MyCampaigns+   Columns 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$ Campaign Name��   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto    MinWidth$	 150Q�   CellTemplate �� .R��� $ 
Horizontal=�$��2q�$��Center=�.+��� $��imgCampaignStatusIcon��$��15��$��15������ *CampaignStatusImageSource+�� $��txtCampaignName��$
��Left=�$��5,0,0,0q����� *CampaignName+  $ Delivery Mode��$	 160 $	 150Q� �� .R��� $��txtDeliveryMode��$��Center=�$��Center=�$��2q����� *DeliveryMode+  $
 Date��$	 160 $	 150Q� �� .R��� $��txtDate��$��Center=�$��Center=�$��2q����� *Date+  $ Description��$ * $	 110Q� �� .R��� $��txtDescription��$��Center=�$��Center=�$��2q����� *Description+   RowStyle ��  �� 
TargetType,  &Microsoft.Windows.Controls.DataGridRow"   �� Triggers P�  P� Property �@ 
IsSelected P� Value . . ���  �� Property � ��  HighlightedBrush �� Value# [ 