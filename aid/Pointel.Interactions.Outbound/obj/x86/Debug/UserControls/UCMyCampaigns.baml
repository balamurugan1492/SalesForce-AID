   M S B A M L   `   `   `  ����!  Pointel.Interactions.Outbound>    8Pointel.Interactions.Outbound.UserControls.UCMyCampaigns   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      :d2http://schemas.microsoft.com/expression/blend/2008  5        � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $V P/Pointel.Interactions.Outbound;component/Resources/StylesResourceDictionary.xaml?�6(   %    5      .��5      �� ���#ADAAAD$��1q�6#   $��5q�67   $
��Auto��6B   $
��Auto��6O   
�������._�5   
   �  � RowDefinitions 5   
   �� 5      $��60 �6   6*   �� 5      $��* �6   6)   .+�5   
   �� -   5        $��btnGetReord��5      $��45��6$   $��55��62   $��55��6@   $��2q�6L      NormalImageButton#��[  6W   $	��Top=�6�   $��Stretch=�6�   $
��Left=�6�   {�6�   �� *IsEnableGetRecord+.��5      ��  �� Orientation$ Vertical=�6   $��Center=�60   .+�5      �� $C��=/Pointel.Interactions.Outbound;component/Images/GetRecord.png��6   $��40��6`   $��40��6k   6y   �� 5      $��Center=�6    �� TextAlignment$ Center=�69   $��
Get Record��6Q   $��Calibri)�6c   $��11(�6x   $
��Wrap=�6�   $��2,0,2,0q�6�   {�6�   �� *IsEnableRelease+5      5       � ToolTip 5      �� 5      
����������#ADAAAD$	��0.5q�6B   $��-5q�6X   ._�5      �  5      �� 5      $
��Auto �6,   6<   �� 5      $��* �6,   69   .+�5!      �� -   5        $��ReleaseHeading��5!   $   $��
Get Record��6:   $��0��6L    �� Padding$ 	10,5,10,0q�6Y   $
��Bold"�6m   $��Calibri)�6   6�   �� 5"      -   5        $��ReleaseContent��5"   $   $��1��6:   $ 
10,2,10,10q�6G   $��WrapWithOverflow=�6\   $��Calibri)�6|   $
��Auto��6�   $	��200��6�   $&�� Get Record from Outbound Contact��6�   5#      5$      5&      )  #Microsoft.Windows.Controls.DataGrid  5'      -   5        $��DGMyCampaigns��5'   |      RowHeaderWidth$ 0Q�6   ���#ADAAAD$��2q�6C   
�������$
��Left=�6a   $	��Top=�5(      $
��Left=�6-   	   AutoGenerateColumns	 .  
   CanUserAddRows
 .     HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   CanUserDeleteRows .  $��1��5)   W      HeadersVisibility$ Column=�6d      SelectionMode$ Single=�6   ��63   �� *MyCampaigns+   Columns 5*      7  1Microsoft.Windows.Controls.DataGridTemplateColumn  5+      /  )Microsoft.Windows.Controls.DataGridColumn   Header$ Campaign Name��60      Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto 6G      MinWidth$	 150Q�6T      CellTemplate 5,      �� 5-      .R�5.   "   �� $ 
Horizontal=�6-   $��2q�6F   $��Center=�6Q   .+�5/   &   �� $��imgCampaignStatusIcon��6,   $��15��6I   $��15��6U   ��6`   �� *CampaignStatusImageSource+�� 50   &   $��txtCampaignName��6K   $
��Left=�60   $��5,0,0,0q�6�   ��6b   �� *CampaignName+6�   51   #   52      54        55      $ Delivery Mode��60   $	 160 6G   $	 150Q�6S    56      �� 57      .R�58   "   �� $��txtDeliveryMode��6I   $��Center=�6,   $��Center=�6a   $��2q�6�   ��6|   �� *DeliveryMode+6�   59      5;        5<      $
 Date��60   $	 160 6>   $	 150Q�6J    5=      �� 5>      .R�5?   "   �� $��txtDate��6I   $��Center=�6,   $��Center=�6Y   $��2q�6�   ��6t   �� *Date+6�   5@      5B        5C      $ Description��60   $ * 6E   $	 110Q�6O    5D      �� 5E      .R�5F   "   �� $��txtDescription��6I   $��Center=�6,   $��Center=�6`   $��2q�6�   ��6{   �� *Description+6�   5G      5I         RowStyle 5K      �� 5L       �� 
TargetType,  &Microsoft.Windows.Controls.DataGridRow"  6    �� Triggers 5M      P� 5N       P� Property �@ 
IsSelected P� Value . . �5O   "   ��  �� Property � ��  HighlightedBrush �� Value# [ 6?   5P      5R      5T      5U      5V      5W      