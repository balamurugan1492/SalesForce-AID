   M S B A M L   `   `   `  ����   Pointel.Interactions.Email=    7Pointel.Interactions.Email.UserControls.DataUserControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6my-http://schemas.microsoft.com/wpf/2008/toolkit  -   $��Calibri)�$��12(�  � 	Resources   �� MergedDictionaries �� Source��  �� $S M/Pointel.Interactions.Email;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� $
��Auto�����#D6D7D6$��1q�._��� -   $��	tabDataUC��$��10��$��0��$��Stretch=�$��2q�.@��� -   $��tbItemCaseData��$
��Auto��$��
Case _Data��$
��Bold"�.��� $��2q� � RowDefinitions �� $��35 ��� $��* � � ColumnDefinitions �� $��* ��� $��40 �.+��� ���#FFC21F$��1q�$��0��$��2�����#FFD66A$��2,2,2,0q��� $
��Left=�$��0��$��0��$
��Left=�$
��Bold"�$��Case Information��$��Center=��� -   $��btnAddCaseData��$��30��$��30�� K� ZIndex$
 1000��$��0,3,3,0q�   NormalImageButton#��[  $��0��$��1��$	��Top=�$��Right=�$��Stretch=�.����  �� Orientation$ Vertical=�$��Center=�.+��� -   $��
imgAddCase��$9��3/Agent.Interaction.Desktop;component/Images/Add.png��$��15��$��15���� $��Center=� �� TextAlignment$ Center=�$	��Add��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Insert Case Data��$��0��	 �� Padding$	 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$	 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$.��(Insert additional case data to this mail���� ���#FFC21F$��1q�$��1��$��2��$��2,0,2,0q�._�� 
�������$
��Auto��.+�)  #Microsoft.Windows.Controls.DataGrid  -   $��DGCaseDataInfo��
   RowHeaderWidth$
 0Q�$��5q�
�������$��Stretch=�$��Stretch=�$��Stretch=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6$
��Auto=�   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .  $
��Auto��$
��Auto��   SelectionMode$ Single=����� *EmailCaseData+ � ContextMenu �� .@�m� -   $
��Copy��$��_Copy�� m� InputGestureText$  �� m� Command$
 Copy��   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$	 180    MinWidth$	 150Q�   MaxWidth$	 200Q�   CanUserSort .   �� ShowOnDisabled .    
IsReadOnly . ���� *
Fontweight+   
FontFamily �� *
Fontfamily+4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Key+   ElementStyle ��   �� 
TargetType"  ��.��� ! �� Property! � ��" �� Value$
" Wrap=��� ! � ��$" 2q��� ! �@��Padding$" 2q�7  1Microsoft.Windows.Controls.DataGridTemplateColumn  $ Value��$	 10* $	 190Q�-   cdValue��#   ClipboardContentBinding# �� *Value+$   CellTemplate$ �� .R��� $��txtValue��$��2q�$	 2q�$
��Wrap=�$
��Left=�$��Normal"�$��Calibri)� . ���� *Value+%   CellEditingTemplate% �� .R�� $��	editStack��$��Stretch=� �� $��* ��� $
��auto ��� $��40 �.+��� $��txtValue��& �� 	UndoLimit$& 10��$��Stretch=�$��2q�$��2q�' �� TextWrapping$
' Wrap=�$��0��  DefaultCopyContextmenu# [ ���� *Value+( �� Mode$( TwoWay=��� $��	btnUpdate��-	   $��1��) � Cursor$
) Hand��$��2,0,2,0q�$
��Left=�$��Center=�$
��Auto��$
��Auto�����#007EDF
������ .���� $ Vertical=�$��Center=�.+��� $:��4/Agent.Interaction.Desktop;component/Images/Edit.png��$��10��$��10�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Update Data��$��0��$	 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$	 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Update the case data.���� $��	btnCancel��-
   $��2��$
) Hand��$��0,0,2,0q�$
��Left=�$��Center=�$
��Auto��$
��Auto�����#007EDF
������ .���� $ Vertical=�$��Center=�.+��� $<��6/Agent.Interaction.Desktop;component/Images/cancel.png��$��10��$��10�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��CancelHeading��$��Cancel��$��0��$	 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��CancelContent��$��1��$	 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$#��Cancel the case data changes.��*   	CellStyle* �� -  'Microsoft.Windows.Controls.DataGridCell"   .��� ! � ��$" 0q�+ �� Triggers+ P� , P� Property, �@ 
IsSelected- P� Value- . . ��� ! � ��  HighlightedBrush#" [ �� ! � ��
" �   �.   RowStyle. �� ,  &Microsoft.Windows.Controls.DataGridRow"   .��� ! � ��$" 0q�+ P� , �@ 
IsSelected- . . ��� ! � ��#" [ �� ! � ��
" �   ��� -   $��
txtMessage��$'��!No case data available to display��$��Center=�$��Center=�$y�	Collapsed=��� -   $��tabitemDisposition��$��_Dispositions��$
��Auto��$
��Bold"�.��� -   $��stpDispCodelist��$��5q�$	��Top=�$��Stretch=��� -   $��tbItemNotes��$��_Note��$
��Bold"�.��� $��Stretch=�$��Stretch=� �� $��10��$��* ��� $��40��$
��Auto � �� $��* �.+��� -   $��txtNotes��$& 10��$��0��$��0��$y�Visible=�$��Stretch=�/ �� AcceptsReturn/ . 0 �� 
AcceptsTab0 . $
��Auto=�$
��Auto=���.   controltempTextbox#��[ $��50��$��2q�$��Normal"�$
' Wrap=�# [ �� -   $��btnSaveNote��$��1��$��0��$��50��$��25��$��Center=�$y�	Collapsed=�$��2,2,5,2q�$��Right=�  NormalButton#��[ $
��Save��$��Normal"�