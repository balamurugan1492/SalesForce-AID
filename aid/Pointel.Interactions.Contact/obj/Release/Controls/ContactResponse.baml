   M S B A M L   `   `   `  ����    Pointel.Interactions.Contact;    5Pointel.Interactions.Contact.Controls.ContactResponse   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -   $��Calibri)�  � 	Resources   �� MergedDictionaries �� Source��  �� $T N/Pointel.Interactions.Contact;component/Resource/StylesResourceDictionary.xaml?�%    .���  � RowDefinitions �� $��40 ��� -    �Name$ ResponseExpand��$��60 ��� $��75 ��� $��* �.+��� $��0��$��2q�$��0�����#D6D7D6$��1q�
������ ._��  � ShowGridLines .  $��37��$	��Top=� � ColumnDefinitions �� $	��155 ��� $��5 ��� $��* ��� $��25 � �� $��4 ��� $
��Auto ��� $��1 ��� $��3* �.+��� -   $��cmbFrom��$��1��   ComboBox#��[  $��Center=�$	��150�� �� SelectedIndex$ 0��$��Center=�$��26��$��2,1,2,2q��� -   $��brdSearchBar��$��0,2,0,2q�$��2��$��1��$��23�����#adaaad$	��0.5q�._��  �� �� $
��Auto �.+��� -   $��	txtSearch�� �� 	UndoLimit$ 10��
������ 
�������$��0,1,0,-1q�
	 � Tag$	 search��.��
 � ToolTip
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��searchResponseHeading��$��search��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��searchContent��$>��8Type text and press search button to search for the text��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.���� -    ��Name$ btnAdvanceSearchResponse��$��1��$��2q�  NormalImageButton#��[  � Cursor$
 Hand��
�������$��20��$
��Left=�$��23��
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -	   $��AdvanceSearchHeading��$��search��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -
   $��AdvanceSearchContent��$>��8Type text and press search button to search for the text��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��.���� $M��G/Agent.Interaction.Desktop;component/Images/Contact/SearchButton-01.png��$��2q�$��1���� -   $ btnAdvanceSearch��#��[ $
 Hand��$��3��$��1��$��20��$
��Left=�$	�� 23��$��3,3,0,3q�.���� -    ��Name$ btnDownload��$��0��$��4������ *EmailIconImageSourceDown+
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��ShowAdvanceSearchHeading��$��Advance search��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��ShowAdvanceSearchContent��$/��)Agent can view the advance search options��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� $��1��$��2q����#D6D7D6$��1q�
������ ._��  �� $
��Auto ��� $	��100 ��� $
��Auto ��� $��* � �� -   $ rowCheckSearch��$��25 ��� -   $ rowCheckSearch1��$��25 �.+��� -   $��	lblSearch��$��0��$��0��$��Center=�$��Search for text in :���� -   $��chkResponseName��
���   � d� 	IsChecked$
 TrueY�$��0��$��2��$��5q�$	��Top=�$��Response Names��$��Calibri)�$��12(��� -   $��chkResponseBody��
���   �$
 TrueY�$��1��$��2��$��5q�$��Center=�$��Response Body Text��$��Calibri)�$��12(��� $��2��$��2q����#D6D7D6$��1q�
������ ._��  �� �� $
��Auto �.+��  �� $
��Auto ��� $��* �.+��� -   $��lblResponseName��$��0��$��Center=�$��View :��$��50���� -   $��cmbResponse��$��1��#��[  $��Center=�$��23��$ 0�����#EDEDED$��0,3,3,3q�.@��� $��All Responses���� $��Favorite Responses��� $��1�� �� $
��Auto ��� $
��265* ��� $
��190* ��� $��20 � .+���  �� Orientation$ 
Horizontal=�.+��� -   $ btnAddResponseCompose��#��[ $
 Hand��$��25��{�.  $
��Left=�.���� -   $ btnFavouriteCompose��$��2q�$O��I/Agent.Interaction.Desktop;component/Images/Contact/InsertResponse-01.png��
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -   $��AddResponseHeading��$��Add Response��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��AddResponseContent��$-��'Add customized text to the current mail��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $ btnAddResponse��#��[ $
 Hand��$��25��$
��Left=�.���� -   $ btnFavourite��$��2q�$T��N/Agent.Interaction.Desktop;component/Images/Contact/AddFavoriteResponse-01.png��
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Add Favorite Response��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $ ��AddFavoriteResponseContent��$1��+Add selected responses to the favorite list��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $ btnRemoveResponse��#��[ $
 Hand��$��25��$��0��.���� -   $ btnRemoveResponse1��$��2q�$O��I/Agent.Interaction.Desktop;component/Images/Contact/RemoveResponse-01.png��
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� $��Remove standard response��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $(��"Agent can remove favorite response��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200���� -   $ btnContactExpand��#��[ $
 Hand��$��5��$��20��.���� -    $ 
btnContact��$��0��$��2��$	�� 20������ *EMailIconImageSourceExpand+
 �� 
����������#ADAAAD$	��0.5q�$��-5q�._��  �� $
��Auto ��� $��* �.+��� -!   $��ExpandHeading��$��Expand��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -"   $��ExpandContent��$��Show details panel��$��1��$ 
10,2,10,10q�$��WrapWithOverflow=�$��Calibri)�$
��Auto��$	��200��� $��3��$��2q�
������� �� $��20 ��� -#   $ 
ExpandGrid��$��* ��� -$   $ ExpandGridAuto��$��* �.+��� $��0��$��0��$
��Auto�����#D6D7D6$��1q�
������ ._��� -%   $��	ListView1��$��0��$��0��
�������$��21�� �� View ��  �� AllowsColumnReorder .  �� ColumnHeaderToolTip$ Response��.P��� $��Response�� �� Width$	  70�� �� DisplayMemberBinding ��  �� Path$
 Name��� $��1��$��0��$
��Auto��$��2q����#D6D7D6$��1q�
�������._�� $��1��$��0��.+��� -&   $��txtAlertMessage��$�� No Data found.��$y�	Collapsed=�R� -'   $��	TreeView1��$	��Top=�$��0q�   �� Color��%  (��       * SystemColors.HighlightBrushKey+)(��      *SystemColors.ControlBrushKey+)(�� (     *$"SystemColors.HighlightTextBrushKey+)(�� 9     *" SystemColors.ControlTextBrushKey+)�� $ #CEE7F7���� $ 	#FFDDDDDD���� $ Black���� $ Black������  �� 
TargetType" Q�.���  �� Property � �� �� Value$	 1.5q� �� Triggers P�  P� Property �@Q�
IsSelected P� Value . . ���  � �� �#adc6e5��  � ��$
 Auto��]�  ]� 
Conditions ��  �� Property �@Q�
IsSelected  �� Value  . ��  �@Q�IsSelectionActive  .  ./���  � ��
 �����! �� 	Resources! ��%9   '��       �� " ��.���  �@��CornerRadius$ 2���� $��2��$��0��$
��Auto��$��2q�
������ ���#D6D7D6$��1q�._�� -(   $��grd_ResponseContent�� �� $
��Auto ��� $
��Auto ��� $��* �.+��� -)   $��
txtSubject��$��2q�$��0��$��Stretch=�$
��Bold"�$
��Auto��$
��Auto��$��WrapWithOverflow=�� -*   $��wpAttachments��$��1��$��2q�$
��Auto��$
��Auto��