   M S B A M L   `   `   `  ����  Pointel.Interactions.Chat<    6Pointel.Interactions.Chat.UserControls.InteractionData   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      :d2http://schemas.microsoft.com/expression/blend/2008  5      -   5        $
��Auto��5   K   $
��Auto��6Y   $��Calibri)�6f   $��12(�6{     � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $R L/Pointel.Interactions.Chat;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      � .+�5   
   �� $
��Auto��6   ���#D6D7D6$��1q�64   ._�5      �� -   5        $��tabDisposition��5      $��Stretch=�6/   $��2q�6K   .@�5      �� -   5        $��
tbItemData��5      $
��Auto��6,   $��
Case _Data��6:   $
��Bold"�6N   .��5      � -   5        $��
gdCaseData��5   &   $��2q�6    � RowDefinitions 5      �� 5      $��35 �6,   6:   �� 5       � ColumnDefinitions 5      �� 5      $	��20* �6/   6=   �� 5      $��40 �6/   6<   .+�5      �� ���#FFC21F$��1q�67   $��0��6K   $��2��6X   ���#FFD66A$��2,2,2,0q�6�   6�   �� 5      $
��Left=�6    $��0��6B   $��0��6O   $
��Left=�6`   $
��Bold"�6{   $��Case Information��6�   $��Center=�6�   �� 5"      -   5        $��btnAddCaseData��5"   !   $��30��67   $��30��6B    K� ZIndex$
 1000��6N   $��0,3,3,0q�6b      NormalImageButton#��[  6s   $��0��6�   $��1��6�   $	��Top=�6�   $��Right=�6  $��Stretch=�6  y�6�   �� *IsChatEnabledAddCaseData+.��5#      ��  �� Orientation$ Vertical=�6)   $��Center=�6@   .+�5$   "   �� -   5        $��
imgAddCase��5$   (   $9��3/Agent.Interaction.Desktop;component/Images/Add.png��6:   $��15��6w   $��15��6�   6�   �� 5%   "   $��Center=�6,    �� TextAlignment$ Center=�6I   $	��Add��6a   $��Calibri)�6l   $��11(�6�   $
��Wrap=�6�   $��2,0,2,0q�6�   5&   #   5'       � ToolTip 5(      �� 5)   "   
����������#ADAAAD$	��0.5q�6R   $��-5q�6h   ._�5*   &   �  5+   *   �� 5,   .   $
��Auto �6<   6L   �� 5-   .   $��* �6<   6I   .+�5/   *   �� $	��Add��65   $��0��6@   	 �� Padding$	 	10,5,10,0q�6M   $
��Bold"�6a   $��Calibri)�6s   6�   �� 50   *   $��1��64   $	 
10,2,10,10q�6A   $��WrapWithOverflow=�6V   $��Calibri)�6v   $
��Auto��6�   $	��200��6�   $!��Insert additional case data��6�   51   '   52   #   54      �� 55      ���#FFC21F$��1q�67   $��2,0,2,0q�6K   $��1��6\   $��2��6i   ._�56      )  #Microsoft.Windows.Controls.DataGrid  -   5        $��DGCaseDataInfo��56   �   
   RowHeaderWidth$
 0Q�6*   $��5q�6=   
�������$   HorizontalScrollBarVisibility$
 Auto=�6[   $��Stretch=�6�   $��Stretch=�6�   $��Stretch=�6�      HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .    ColumnWidth8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto 58   \   $
��Auto��6�   $
��Auto��6�   "   VerticalScrollBarVisibility$
 Auto=�6�   ��56   �   �� *NotifyCaseData+ � ContextMenu 59   "   �� 5:   &   .@�5;   *   m� -   5        $
��Copy��5;   B   $��_Copy��63    m� InputGestureText$  ��6N    m� Command$
 Copy��6b   5<   '      Columns 5>   "   3  -Microsoft.Windows.Controls.DataGridTextColumn  5?   &   /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��6<      Width$	 150 6�      MinWidth$	 120Q�6�      MaxWidth$	 180Q�6�    �� ShowOnDisabled .    
IsReadOnly . ��6I   �� *
Fontweight+   
FontFamily 6u   �� *
Fontfamily+4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding 6�   �� *Key+    ElementStyle  5@   *   �� 5A   .   ! �� 
TargetType"! ��64   .�5B   2   �� " �� Property" � ��# �� Value$
# Wrap=�6[   �� 5C   2   " � ��$# 2q�6K   �� 5D   2   " �@��Padding$# 2q�6L   5E   /   5G   '   7  1Microsoft.Windows.Controls.DataGridTemplateColumn  5J   &   $ Value��6@   $	 10* 6O   $	 190Q�6�   -	   5        cdValue��$   ClipboardContentBinding$ 5J   [   �� *Value+%   CellTemplate% 5K   *   �� 5L   .   .R�5M   2   �� $��txtValue��6�   $��2q�6<   $	 2q�6G   $
��Wrap=�6S   $
��Left=�6g   $��Normal"�6�   $��Calibri)�6�    . ��6�   �� *Value+5N   3   5O   /   &   CellEditingTemplate& 5Q   *   �� 5R   .   .R�5S   2   � $��	editStack��67   $��Stretch=�6H    5T   6   �� 5U   :   $��* �6K   6W   �� 5V   :   $
��Auto �6K   6Z   �� 5W   :   $��40 �6K   6X   .+�5Y   6   �� $��txtValue��6�   ' �� 	UndoLimit$' 10��6>   $��2q�6M   $��2q�6X   $��0��6d   ( �� TextWrapping$
( Wrap=�6t   $��Stretch=�6�     DefaultCopyContextmenu# [ 6�   $	��100��6  ��6�   �� *Value+) �� Mode$) TwoWay=�6  �� 5j   6   $��	btnUpdate��6�   -
   5        $��1��5j   =   * � Cursor$
* Hand��6N   $��2,0,2,0q�6\   $
��Left=�6n   $��Center=�6�   $��15��6�   $��15��6�   ���#007EDF
������ .��5k   :   �� $ Vertical=�6E   $��Center=�6\   .+�5l   >   �� $:��4/Agent.Interaction.Desktop;component/Images/Edit.png��6D   $��10��6�   $��10��6�   6�   5m   ;    5n   :   �� 5o   >   
����������#ADAAAD$	��0.5q�6n   $��-5q�6�   ._�5p   B   �  5q   F   �� 5r   J   $
��Auto �6X   6h   �� 5s   J   $��* �6X   6e   .+�5u   F   �� $��UpdateHeading��6P   $��Update��6e   $��0��6s   $	 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   6�   �� 5v   F   $��UpdateContent��6P   $��1��6e   $	 
10,2,10,10q�6r   $��WrapWithOverflow=�6�   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $��Agent can update data.��6�   5w   C   5x   ?   5z   7   �� 5{   6   $��	btnCancel��6�   -   5        $��2��5{   =   $
* Hand��6M   $��2,0,2,0q�6[   $
��Left=�6m   $��Center=�6�   $��15��6�   $��15��6�   ���#007EDF
������ .��5|   :   �� $ Vertical=�6E   $��Center=�6\   .+�5}   >   �� $<��6/Agent.Interaction.Desktop;component/Images/cancel.png��6D   $��10��6�   $��10��6�   6�   5~   ;    5   :   �� 5�   >   
����������#ADAAAD$	��0.5q�6n   $��-5q�6�   ._�5�   B   �  5�   F   �� 5�   J   $
��Auto �6X   6h   �� 5�   J   $��* �6X   6e   .+�5�   F   �� $��CancelHeading��6P   $��Cancel��6e   $��0��6s   $	 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   6�   �� 5�   F   $��CancelContent��6P   $��1��6e   $	 
10,2,10,10q�6r   $��WrapWithOverflow=�6�   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $��Agent can cancel data.��6�   5�   C   5�   ?   5�   7   5�   3   5�   /   +   	CellStyle+ 5�   *   �� 5�   .   -  'Microsoft.Windows.Controls.DataGridCell"!  64   .�5�   2   �� " � ��$# 0q�6T   , �� Triggers, 5�   2   P� 5�   6   - P� Property- �@ 
IsSelected. P� Value. . . �5�   :   �� " � ��  HighlightedBrush## [ 6W   �� 5�   :   " � ��
# �   �6W   5�   7   5�   /   5�   '   /   RowStyle/ 5�   "   �� 5�   &   ,  &Microsoft.Windows.Controls.DataGridRow"!  6,   .�5�   *   �� " � ��$# 0q�6L   , 5�   *   P� 5�   .   - �@ 
IsSelected. . . �5�   2   �� " � ��## [ 6O   �� 5�   2   " � ��
# �   �6O   5�   /   5�   '   5�      5�      5�      5�      �� 5�      -   5        $��tabitemDisposition��5�   ?   $��_Dispositions��6   $
��Auto��61   $
��Bold"�6Y   .��5�      � -   5        $��stpDispCodelist��5�   &   $��5q�6   $	��Top=�6=   $��Stretch=�6U   5�      5�      �� 5�      -   5        $��tbItemNotes��5�      $��_Note��6-   $
��Bold"�6<   $
��Auto��6N   .��5�      � -   5        $��gdNote��5�   U   $��Stretch=�6   $��Stretch=�67    5�      �� 5�      $��* �6-   6:   �� 5�      $
��Auto �6-   6=   .+�5�      �� -   5        $��txtNotes��5�   2   $' 10��6"   $��0��6B   $��0��6O   $y�Visible=�6_   $��Stretch=�6t   0 �� AcceptsReturn0 . 1 �� 
AcceptsTab1 . $
��Auto=�6�   $
��Auto=�5�   m   ��.   controltempTextbox#��[ 6�   $��50��6�   $��2q�6   $��Normal"�6  $
( Wrap=�6  ��68   �� *InteractionNoteContent+$) TwoWay=�.��6V    5�        HorizontalScrollBar  VerticalScrollBar��%  '��       7  7  �� 5�   "   "! ��6(   .�5�   &   �� " �@J�SnapsToDevicePixels# . 6L   �� 5�   &   " �@�OverridesDefaultStyle# . 6N   , 5�   &   P� 5�   *   - �@��Orientation$. 
Horizontal=�6I   . �5�   .   �� " � ��$
# Auto��6F   �� 5�   .   " � ��$# 18��6G   �� 5�   .   " � ��8#   6I   5�   +   P� 5�   *   - �@��Orientation$. Vertical=�6I   . �5�   .   �� " � ��$# 18��6F   �� 5�   .   " � ��$
# Auto��6G   �� 5�   .   " � ��8#  6I   5�   +   5�   #   5�      .��61   5�      �� 5�      -   5        $��btnSaveNote��5�   !   $��1��64   $��0��6A   $��50��6Q   $��25��6\   $��Center=�6h   $��2,2,5,2q�6�   $��Right=�6�     NormalButton#��[ 6�   $
��Save��6�   $��Normal"�6�   6�   5�      5�      5�      5�      5�      5�      