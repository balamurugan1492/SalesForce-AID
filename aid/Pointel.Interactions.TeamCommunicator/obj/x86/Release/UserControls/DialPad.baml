   M S B A M L   `   `   `  ���� )  %Pointel.Interactions.TeamCommunicator@    :Pointel.Interactions.TeamCommunicator.UserControls.DialPad   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     6my-http://schemas.microsoft.com/wpf/2008/toolkit  -   $��TeamCommunicator��  � 	Resources   �� MergedDictionaries �� Source   GroupHeaderStyle �� 
TargetType �� Property �� Value �� 
TargetType 9�Name 9� 
IsExpanded��  �� $^ X/Pointel.Interactions.TeamCommunicator;component/Resources/StylesResourceDictionary.xaml?�%�   &	        �� " ��.���  � �� �� " ��.R�9� $	 exp�� . ���#007edf
����������� $��Calibri)�$
��Bold"�$��Normal&�
����������� *Name+.���� .��� $
��Auto��.+�� -   $��mainGrid��$	��209��$	��300��
�������$��Stretch=�	 � RowDefinitions	 �� $
��Auto ��� $��* �.+��� -   $��	txtSearch��
 �� 	UndoLimit$
 10��$��Stretch=�$	��Top=�$��5q�  controltempTextbox#��[ $��0��� -   $��	TeamCGrid��$��Stretch=�$��1��$y�Visible=�.+��� 
�������._�� -   $��ToolGrid��	 �� $
��Auto ��� $��* ��� $
��Auto �.+���  �� Orientation$ 
Horizontal=�$
��Auto��$��Stretch=�.+�d� -   $��btnSelectAll��$��Right=�$��2q�  
SideButton#��[ .���� $;��5/Agent.Interaction.Desktop;component/Images/Agent.png��$��15��$��20�� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��
Search All��$��0�� �� Padding$ 	10,5,10,5q�$��Calibri)�d� -   $��btnFavourite��$��Right=�$��2q�#��[ .���� $?��9/Agent.Interaction.Desktop;component/Images/Favourite.png��$��15��$��20�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $"��Show and search my favorites��$��0��$ 	10,5,10,5q�$��Calibri)�d� -   $��	btnRecent��$��Right=�$��2q�#��[ .���� $:��4/Agent.Interaction.Desktop;component/Images/Time.png��$��15��$��20�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��Show and search recent��$��0��$ 	10,5,10,5q�$��Calibri)��� -	   $��	comboBox1��$��23�� �� SelectedIndex$ 0��$	��100��$��2q� �� Text �� *SelectedType+���� *
FilterList+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��Select a type��$��0��$ 	10,5,10,5q�$��Calibri)�d� -
   $��btnGroup��$��Right=�$��2q�#��[ .���� -   $��imgGroup��$;��5/Agent.Interaction.Desktop;component/Images/Group.png��$��15��$��20�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��
Show types��$��0��$ 	10,5,10,5q�$��Calibri)�)  #Microsoft.Windows.Controls.DataGrid  -   $��DGTeamCommunicator��$��Stretch=�$   HorizontalScrollBarVisibility$ Disabled=�"   VerticalScrollBarVisibility$
 Auto=�   RowHeaderWidth$ 0Q�$��5q�
�������$��Stretch=�$
��Left=�   HorizontalGridLinesBrush �#D6D7D6   GridLinesVisibility$
 None=�   VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .  $��1��   HeadersVisibility$
 None=�����  �� 
GroupStyle (  "System.Windows.Controls.GroupStyle     ContainerStyle# [     Panel �� .R�A  ;Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter     Columns 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto     MinWidth$  30Q�!   CellTemplate! �� .R��� $ 
Horizontal=�" �� 	IsEnabled" . # �� HasDropShadow# .   TCStackPannelImageButton#��[ $��5q�.+��� $��btnCall��-   " .   NormalImageButton#��[  �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��0��$ 	10,5,10,5q�$��Calibri)����� *ImageToolTip+.���� $��imgMessageIcon��$��15��$��15������ *SearchIconImageSource+�� $��
btnContext��-   " .   TCArrowImageButton#��[ $��Bottom=� �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��Action Menu��$��0��$ 	10,5,10,5q�$��Calibri)�.���� $��imgicon��$;��5/Agent.Interaction.Desktop;component/Images/Eject.png��$��2,0,0,4q�$��11��$��11��  $ * ! �� .R��� $ Vertical=�$��0,5,0,5q�.+��� $��txtMessageSender��$
��Left=�
���   �$	��175��$��2,0,0,0q����� *SearchedName+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��0��$ 	10,5,10,5q�$��Calibri)����� *SearchedName+�� $ 
Horizontal=�.+��� $��imgicon��$��15��$��15������ *StatusImageSource+�� $��txtMessageSender1��$
��Left=�
���   �$��2,0,0,0q����� *Status+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $ 
Horizontal=�$��5q�.+��� $��15��$��15������ *MediaImageSource+�� $��Calibri)�$��5,0,0,0q�$��Center=����� *	MediaType+�� $��imgstatusicon��$��20,0,0,0q�$��10��$��10������ *StatusImageSource+�� $��Calibri)�$��Center=�$��2,0,0,0q����� *Status+  $ 25 ! �� .R��� $��
imgFavicon��$��15��$��15������ *FavoriteImageSource+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��0��$ 	10,5,10,5q�$��Calibri)����� *FavoriteToolTip+  $ 45 ! �� .R��� $��imgicon��$��20��$��0,0,19,0q�$��20������ *DetailImageSource+ �� 
����������#ADAAAD$	��0.5q�$��-5q�._�� 	 �� $
��Auto �.+��� $��0��$ 	10,5,10,5q�$��Calibri)����� *	ToolTip+�� $ Vertical=�$
��Auto��$��3��.+��� $ 
Horizontal=�.+��� $��0��$��10,5,5,0q�$��25��$��Calibri)����� *InternalTargets+�� -   $��txtErrorMessage��$"��Stat Server is not available��$��Calibri)�$��12(�$��Center=�$��Center=�