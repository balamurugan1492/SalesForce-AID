   M S B A M L   `   `   `  ����  Pointel.Interactions.Email=    7Pointel.Interactions.Email.UserControls.DataUserControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      -   5        $��Calibri)�5   H   $��12(�6]     � 	Resources  5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $S M/Pointel.Interactions.Email;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      � .+�5   
   �� $
��Auto��6   ���#D6D7D6$��1q�68   ._�5      �� -   5        $��	tabDataUC��5      $��10��6*   $��0��68   $��Stretch=�6I   $��2q�6e   .@�5      �� -   5        $��tbItemCaseData��5      $
��Auto��60   $��
Case _Data��6>   $
��Bold"�6R   .��5      � $��2q�6    � RowDefinitions 5      �� 5      $��35 �6,   6:   �� 5      $��* �6,   6:    � ColumnDefinitions 5      �� 5      $��* �6/   6;   �� 5      $��40 �6/   6<   .+�5      �� ���#FFC21F$��1q�67   $��0��6K   $��2��6X   ���#FFD66A$��2,2,2,0q�6�   5      �� 5      $
��Left=�6    $��0��6B   $��0��6O   $
��Left=�6`   $
��Bold"�6{   $��Case Information��6�   $��Center=�6�   �� 5       -   5        $��btnAddCaseData��5    !   $��30��67   $��30��6B    K� ZIndex$
 1000��6N   $��0,3,3,0q�6b      NormalImageButton#��[  6s   $��0��6�   $��1��6�   $	��Top=�6�   $��Right=�6�   $��Stretch=�6�   .��5!      ��  �� Orientation$ Vertical=�6)   $��Center=�6@   .+�5"   "   �� -   5        $��
imgAddCase��5"   (   $9��3/Agent.Interaction.Desktop;component/Images/Add.png��6:   $��15��6w   $��15��6�   6�   �� 5#   "   $��Center=�6,    �� TextAlignment$ Center=�6I   $	��Add��6a   $��Calibri)�6l   $��11(�6�   $
��Wrap=�6�   $��2,0,2,0q�6�   5$   #   5%       � ToolTip 5&      �� 5'   "   
����������#ADAAAD$	��0.5q�6R   $��-5q�6h   ._�5(   &   �  5)   *   �� 5*   .   $
��Auto �6<   6L   �� 5+   .   $��* �6<   6I   .+�5-   *   �� $��Insert Case Data��65   $��0��6M   	 �� Padding$	 	10,5,10,0q�6Z   $
��Bold"�6n   $��Calibri)�6�   6�   �� 5.   *   $��1��64   $	 
10,2,10,10q�6A   $��WrapWithOverflow=�6V   $��Calibri)�6v   $
��Auto��6�   $	��200��6�   $.��(Insert additional case data to this mail��6�   5/   '   50   #   52      �� 53      ���#FFC21F$��1q�67   $��1��6K   $��2��6X   $��2,0,2,0q�6l   ._�54      � 
�������$
��Auto��66   .+�55   "   )  #Microsoft.Windows.Controls.DataGrid  -   5        $��DGCaseDataInfo��55   }   
   RowHeaderWidth$
 0Q�6.   $��5q�6A   
�������$��Stretch=�6_   $��Stretch=�6�   $��Stretch=�6�      HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6$
��Auto=�56   R      AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .  $
��Auto��58   ^   $
��Auto��6l      SelectionMode$ Single=�6y   ��55   �   �� *EmailCaseData+ � ContextMenu 59   &   �� 5:   *   .@�5;   .   m� -   5        $
��Copy��5;   F   $��_Copy��67    m� InputGestureText$  ��6R    m� Command$
 Copy��6f   5<   +      Columns 5>   &   3  -Microsoft.Windows.Controls.DataGridTextColumn  5?   *   /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��6@      Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$	 180 6�      MinWidth$	 150Q�6�      MaxWidth$	 200Q�6�      CanUserSort .   �� ShowOnDisabled .    
IsReadOnly . ��6M   �� *
Fontweight+   
FontFamily 6y   �� *
Fontfamily+4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding 6�   �� *Key+   ElementStyle 5@   .   �� 5A   2     �� 
TargetType"  ��68   .�5B   6   �� ! �� Property! � ��" �� Value$
" Wrap=�6_   �� 5C   6   ! � ��$" 2q�6O   �� 5D   6   ! �@��Padding$" 2q�6P   5E   3   5G   +   7  1Microsoft.Windows.Controls.DataGridTemplateColumn  5J   *   $ Value��6D   $	 10* 6S   $	 190Q�6�   -   5        cdValue��#   ClipboardContentBinding# 5J   _   �� *Value+$   CellTemplate$ 5K   .   �� 5L   2   .R�5M   6   �� $��txtValue��6�   $��2q�6@   $	 2q�6K   $
��Wrap=�6W   $
��Left=�6k   $��Normal"�6�   $��Calibri)�6�    . ��6�   �� *Value+5N   7   5O   3   %   CellEditingTemplate% 5Q   .   �� 5R   2   .R�5S   6   � $��	editStack��6;   $��Stretch=�6L    5T   :   �� 5U   >   $��* �6O   6[   �� 5V   >   $
��auto �6O   6^   �� 5W   >   $��40 �6O   6\   .+�5Y   :   �� $��txtValue��6o   & �� 	UndoLimit$& 10��6B   $��Stretch=�6Q   $��2q�6   $��2q�6�   ' �� TextWrapping$
' Wrap=�6�   $��0��6�     DefaultCopyContextmenu# [ 6�   ��6�   �� *Value+( �� Mode$( TwoWay=�6�   �� 5Z   :   $��	btnUpdate��6�   -	   5        $��1��5Z   A   ) � Cursor$
) Hand��6Q   $��2,0,2,0q�6_   $
��Left=�6q   $��Center=�6�   $
��Auto��6�   $
��Auto��6�   ���#007EDF
������ .��5[   >   �� $ Vertical=�6I   $��Center=�6`   .+�5\   B   �� $:��4/Agent.Interaction.Desktop;component/Images/Edit.png��6H   $��10��6�   $��10��6�   6�   5]   ?    5^   >   �� 5_   B   
����������#ADAAAD$	��0.5q�6r   $��-5q�6�   ._�5`   F   �  5a   J   �� 5b   N   $
��Auto �6\   6l   �� 5c   N   $��* �6\   6i   .+�5e   J   �� $��Update Data��6T   $��0��6g   $	 	10,5,10,0q�6t   $
��Bold"�6�   $��Calibri)�6�   6�   �� 5f   J   $��1��6T   $	 
10,2,10,10q�6a   $��WrapWithOverflow=�6v   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $��Update the case data.��6�   5g   G   5h   C   5j   ;   �� 5k   :   $��	btnCancel��6�   -
   5        $��2��5k   A   $
) Hand��6Q   $��0,0,2,0q�6_   $
��Left=�6q   $��Center=�6�   $
��Auto��6�   $
��Auto��6�   ���#007EDF
������ .��5l   >   �� $ Vertical=�6I   $��Center=�6`   .+�5m   B   �� $<��6/Agent.Interaction.Desktop;component/Images/cancel.png��6H   $��10��6�   $��10��6�   6�   5n   ?    5o   >   �� 5p   B   
����������#ADAAAD$	��0.5q�6r   $��-5q�6�   ._�5q   F   �  5r   J   �� 5s   N   $
��Auto �6\   6l   �� 5t   N   $��* �6\   6i   .+�5v   J   �� $��CancelHeading��6T   $��Cancel��6i   $��0��6w   $	 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   6�   �� 5w   J   $��CancelContent��6T   $��1��6i   $	 
10,2,10,10q�6v   $��WrapWithOverflow=�6�   $��Calibri)�6�   $
��Auto��6�   $	��200��6�   $#��Cancel the case data changes.��6�   5x   G   5y   C   5{   ;   5|   7   5}   3   *   	CellStyle* 5   .   �� 5�   2   -  'Microsoft.Windows.Controls.DataGridCell"   68   .�5�   6   �� ! � ��$" 0q�6X   + �� Triggers+ 5�   6   P� 5�   :   , P� Property, �@ 
IsSelected- P� Value- . . �5�   >   �� ! � ��  HighlightedBrush#" [ 6[   �� 5�   >   ! � ��
" �   �6[   5�   ;   5�   3   5�   +   .   RowStyle. 5�   &   �� 5�   *   ,  &Microsoft.Windows.Controls.DataGridRow"   60   .�5�   .   �� ! � ��$" 0q�6P   + 5�   .   P� 5�   2   , �@ 
IsSelected- . . �5�   6   �� ! � ��#" [ 6S   �� 5�   6   ! � ��
" �   �6S   5�   3   5�   +   5�   #   �� 5�   "   -   5        $��
txtMessage��5�   ,   $'��!No case data available to display��6>   $��Center=�6g   $��Center=�6�   $y�	Collapsed=�6�   6�   5�      5�      5�      5�      �� 5�      -   5        $��tabitemDisposition��5�   ?   $��_Dispositions��6   $
��Auto��61   $
��Bold"�6Y   .��5�      � -   5        $��stpDispCodelist��5�   &   $��5q�6   $	��Top=�6=   $��Stretch=�6U   5�      5�      �� 5�      -   5        $��tbItemNotes��5�      $��_Note��6-   $
��Bold"�6<   .��5�      � $��Stretch=�6   $��Stretch=�67    5�      �� 5�      $��10��6,   $��* �6;   6H   �� 5�      $��40��6,   $
��Auto �6;   6K    5�      �� 5�      $��* �6/   6;   .+�5�      �� -   5        $��txtNotes��5�   2   $& 10��6"   $��0��6B   $��0��6O   $y�Visible=�6_   $��Stretch=�6t   / �� AcceptsReturn/ . 0 �� 
AcceptsTab0 . $
��Auto=�6�   $
��Auto=�5�   5   ��.   controltempTextbox#��[ 6�   $��50��6�   $��2q�6�   $��Normal"�6�   $
' Wrap=�6�   # [ 6  6W  �� 5�      -   5        $��btnSaveNote��5�   !   $��1��64   $��0��6A   $��50��6Q   $��25��6\   $��Center=�6h   $y�	Collapsed=�6�   $��2,2,5,2q�6�   $��Right=�6�     NormalButton#��[ 6�   $
��Save��6�   $��Normal"�6�   5�      5�      5�      5�      5�      5�      5�      