   M S B A M L   `   `   `  ����   Pointel.Interactions.Chat<    6Pointel.Interactions.Chat.UserControls.InteractionData   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6my-http://schemas.microsoft.com/wpf/2008/toolkit  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  :d2http://schemas.microsoft.com/expression/blend/2008  -   $
��Auto��$
��Auto��$��Calibri)�$��12(�  � 	Resources   �� MergedDictionaries �� Source��  �� $R L/Pointel.Interactions.Chat;component/Resources/StylesResourceDictionary.xaml?�%    .��� .+��� $
��Auto�����#D6D7D6$��1q�._��� -   $��tabDisposition��$��Stretch=�$��2q�.@��� -   $��
tbItemData��$
��Auto��$��
Case _Data��$
��Bold"�.��� -   $��
gdCaseData��$��2q� � RowDefinitions �� $��35 ���  � ColumnDefinitions �� $	��20* ��� $��40 �.+��� ���#FFC21F$��1q�$��0��$��2�����#FFD66A$��2,2,2,0q��� $
��Left=�$��0��$��0��$
��Left=�$
��Bold"�$��Case Information��$��Center=��� -   $��btnAddCaseData��$��30��$��30�� K� ZIndex$
 1000��$��0,3,3,0q�   NormalImageButton#��[  $��0��$��1��$	��Top=�$��Right=�$��Stretch=�y��� *IsChatEnabledAddCaseData+.����  �� Orientation$ Vertical=�$��Center=�.+��� -   $��
imgAddCase��$9��3/Agent.Interaction.Desktop;component/Images/Add.png��$��15��$��15���� $��Center=� �� TextAlignment$ Center=�$	��Add��$��Calibri)�$��11(�$
��Wrap=�$��2,0,2,0q� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $	��Add��$��0��	 �� Padding$	 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$	 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$!��Insert additional case data���� ���#FFC21F$��1q�$��2,0,2,0q�$��1��$��2��._�)  #Microsoft.Windows.Controls.DataGrid  -   $��DGCaseDataInfo��
   RowHeaderWidth$
 0Q�$��5q�
�������$   HorizontalScrollBarVisibility$
 Auto=�$��Stretch=�$��Stretch=�$��Stretch=�   HorizontalGridLinesBrush �#D6D7D6   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .    ColumnWidth8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto $
��Auto��$
��Auto��"   VerticalScrollBarVisibility$
 Auto=����� *NotifyCaseData+ � ContextMenu �� .@�m� -   $
��Copy��$��_Copy�� m� InputGestureText$  �� m� Command$
 Copy��   Columns 3  -Microsoft.Windows.Controls.DataGridTextColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$	 Key��   Width$	 150    MinWidth$	 120Q�   MaxWidth$	 180Q� �� ShowOnDisabled .    
IsReadOnly . ���� *
Fontweight+   
FontFamily �� *
Fontfamily+4  .Microsoft.Windows.Controls.DataGridBoundColumn   Binding �� *Key+    ElementStyle  �� ! �� 
TargetType"! ��.��� " �� Property" � ��# �� Value$
# Wrap=��� " � ��$# 2q��� " �@��Padding$# 2q�7  1Microsoft.Windows.Controls.DataGridTemplateColumn  $ Value��$	 10* $	 190Q�-	   cdValue��$   ClipboardContentBinding$ �� *Value+%   CellTemplate% �� .R��� $��txtValue��$��2q�$	 2q�$
��Wrap=�$
��Left=�$��Normal"�$��Calibri)� . ���� *Value+&   CellEditingTemplate& �� .R�� $��	editStack��$��Stretch=� �� $��* ��� $
��Auto ��� $��40 �.+��� $��txtValue��' �� 	UndoLimit$' 10��$��2q�$��2q�$��0��( �� TextWrapping$
( Wrap=�$��Stretch=�  DefaultCopyContextmenu# [ $	��100������ *Value+) �� Mode$) TwoWay=��� $��	btnUpdate��-
   $��1��* � Cursor$
* Hand��$��2,0,2,0q�$
��Left=�$��Center=�$��15��$��15�����#007EDF
������ .���� $ Vertical=�$��Center=�.+��� $:��4/Agent.Interaction.Desktop;component/Images/Edit.png��$��10��$��10�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��UpdateHeading��$��Update��$��0��$	 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��UpdateContent��$��1��$	 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Agent can update data.���� $��	btnCancel��-   $��2��$
* Hand��$��2,0,2,0q�$
��Left=�$��Center=�$��15��$��15�����#007EDF
������ .���� $ Vertical=�$��Center=�.+��� $<��6/Agent.Interaction.Desktop;component/Images/cancel.png��$��10��$��10�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��CancelHeading��$��Cancel��$��0��$	 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��CancelContent��$��1��$	 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��$��Agent can cancel data.��+   	CellStyle+ �� -  'Microsoft.Windows.Controls.DataGridCell"!  .��� " � ��$# 0q�, �� Triggers, P� - P� Property- �@ 
IsSelected. P� Value. . . ��� " � ��  HighlightedBrush## [ �� " � ��
# �   �/   RowStyle/ �� ,  &Microsoft.Windows.Controls.DataGridRow"!  .��� " � ��$# 0q�, P� - �@ 
IsSelected. . . ��� " � ��## [ �� " � ��
# �   ��� -   $��tabitemDisposition��$��_Dispositions��$
��Auto��$
��Bold"�.��� -   $��stpDispCodelist��$��5q�$	��Top=�$��Stretch=��� -   $��tbItemNotes��$��_Note��$
��Bold"�$
��Auto��.��� -   $��gdNote��$��Stretch=�$��Stretch=� �� $��* ��� $
��Auto �.+��� -   $��txtNotes��$' 10��$��0��$��0��$y�Visible=�$��Stretch=�0 �� AcceptsReturn0 . 1 �� 
AcceptsTab1 . $
��Auto=�$
��Auto=���.   controltempTextbox#��[ $��50��$��2q�$��Normal"�$
( Wrap=����� *InteractionNoteContent+$) TwoWay=�.��    HorizontalScrollBar  VerticalScrollBar��%M  '��       7  7  �� "! ��.��� " �@J�SnapsToDevicePixels# . �� " �@�OverridesDefaultStyle# . , P� - �@��Orientation$. 
Horizontal=�. ��� " � ��$
# Auto���� " � ��$# 18���� " � ��8#   P� - �@��Orientation$. Vertical=�. ��� " � ��$# 18���� " � ��$
# Auto���� " � ��8#  .���� -   $��btnSaveNote��$��1��$��0��$��50��$��25��$��Center=�$��2,2,5,2q�$��Right=�  NormalButton#��[ $
��Save��$��Normal"�