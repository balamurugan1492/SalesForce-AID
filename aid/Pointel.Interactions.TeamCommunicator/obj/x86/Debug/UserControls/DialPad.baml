   M S B A M L   `   `   `  ����)  %Pointel.Interactions.TeamCommunicator@    :Pointel.Interactions.TeamCommunicator.UserControls.DialPad   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      -   5        $��TeamCommunicator��5        � 	Resources  5       �� MergedDictionaries �� Source   GroupHeaderStyle �� 
TargetType �� Property �� Value �� 
TargetType 9�Name 9� 
IsExpanded�� 5	   
    5
      �� 5      $^ X/Pointel.Interactions.TeamCommunicator;component/Resources/StylesResourceDictionary.xaml?�6%   %Q  &	        �� 5      " ��6-   .�5      ��  � �� 5      �� 5      " ��6*   .R�5      9� $	 exp��6'    . ���#007edf
���������5   "   �� 5   &   $��Calibri)�6F   $
��Bold"�6[   $��Normal&�6n   
���������60   �� *Name+5   '   .��5   "   �� 5      5      5      5      5       .��5"      � $
��Auto��6   .+�5#   
   � -   5        $��mainGrid��5#      $	��209��6   $	��300��6+   
�������$��Stretch=�6K   	 � RowDefinitions	 5$      �� 5%      $
��Auto �6    �� 5&      $��* �6    .+�5(      �� -   5        $��	txtSearch��5(   %   
 �� 	UndoLimit$
 10��6   $��Stretch=�66   $	��Top=�6T   $��5q�6l     controltempTextbox#��[ 6w   $��0��5)      6%   � 5*      -   5        $��	TeamCGrid��5*   <   $��Stretch=�6   $��1��6/   $y�Visible=�6M   .+�5+      �� 
�������._�5,      � -   5        $��ToolGrid��5,      	 5-      �� 5.      $
��Auto �6,   6<   �� 5/      $��* �6,   69   �� 50      $
��Auto �6,   6<   .+�52      ��  �� Orientation$ 
Horizontal=�6%   $
��Auto��6>   $��Stretch=�6K   .+�53      d� -   5        $��btnSelectAll��53   R   $��Right=�6+   $��2q�6G     
SideButton#��[ 6�   .��54   "   �� $;��5/Agent.Interaction.Desktop;component/Images/Agent.png��6(   $��15��6g   $��20��6r    � ToolTip 55   "   �� 56   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�57   *   � 	 58   .   �� 59   2   $
��Auto �6@   6P   .+�5;   .   �� $��
Search All��68   $��0��6J    �� Padding$ 	10,5,10,5q�6W   $��Calibri)�6k   6�   5<   +   5=   '   5?      d� 5@      -   5        $��btnFavourite��5@   R   $��Right=�6+   $��2q�6G   #��[ 6�   .��5A   "   �� $?��9/Agent.Interaction.Desktop;component/Images/Favourite.png��6(   $��15��6k   $��20��6v    5B   "   �� 5C   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5D   *   � 	 5E   .   �� 5F   2   $
��Auto �6@   6P   .+�5H   .   �� $"��Show and search my favorites��68   $��0��6\   $ 	10,5,10,5q�6i   $��Calibri)�6}   6�   5I   +   5J   '   5L      d� 5M      -   5        $��	btnRecent��5M   R   $��Right=�6+   $��2q�6G   #��[ 6{   .��5N   "   �� $:��4/Agent.Interaction.Desktop;component/Images/Time.png��6(   $��15��6f   $��20��6q    5O   "   �� 5P   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5Q   *   � 	 5R   .   �� 5S   2   $
��Auto �6@   6P   .+�5U   .   �� $��Show and search recent��68   $��0��6V   $ 	10,5,10,5q�6c   $��Calibri)�6w   6�   5V   +   5W   '   5Y      �� 5Z      -	   5        $��	comboBox1��5Z   E   $��23��6'    �� SelectedIndex$ 0��63   $	��100��6V   $��2q�6b    �� Text 6�   �� *SelectedType+��6�   �� *
FilterList+ 5[   "   �� 5\   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5]   *   � 	 5^   .   �� 5_   2   $
��Auto �6@   6P   .+�5a   .   �� $��Select a type��68   $��0��6M   $ 	10,5,10,5q�6Z   $��Calibri)�6n   6�   5b   +   5c   '   5e      d� 5f      -
   5        $��btnGroup��5f   R   $��Right=�6+   $��2q�6G   #��[ 6y   .��5g   "   �� -   5        $��imgGroup��5g   (   $;��5/Agent.Interaction.Desktop;component/Images/Group.png��68   $��15��6w   $��20��6�    5h   "   �� 5i   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5j   *   � 	 5k   .   �� 5l   2   $
��Auto �6@   6P   .+�5n   .   �� $��
Show types��68   $��0��6J   $ 	10,5,10,5q�6W   $��Calibri)�6k   6�   5o   +   5p   '   5r      5s      )  #Microsoft.Windows.Controls.DataGrid  5t      -   5        $��DGTeamCommunicator��5u   %   $��Stretch=�5t   &   $   HorizontalScrollBarVisibility$ Disabled=�6B   "   VerticalScrollBarVisibility$
 Auto=�6k      RowHeaderWidth$ 0Q�6�   $��5q�6�   
�������$��Stretch=�6�   $
��Left=�5u   ?      HorizontalGridLinesBrush �#D6D7D6   GridLinesVisibility$
 None=�6�      VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .  $��1��5w   X      HeadersVisibility$
 None=�6e   ��6@   ��  �� 
GroupStyle 5x      (  "System.Windows.Controls.GroupStyle  5y   "      ContainerStyle# [  6-      Panel 5{   &   �� 5|   *   .R�5}   .   A  ;Microsoft.Windows.Controls.Primitives.DataGridRowsPresenter  5~   +   5�   #      Columns 5�      7  1Microsoft.Windows.Controls.DataGridTemplateColumn  5�   "   /  )Microsoft.Windows.Controls.DataGridColumn   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto 6=       MinWidth$  30Q�6J   !   CellTemplate! 5�   &   �� 5�   *   .R�5�   .   �� $ 
Horizontal=�69   " �� 	IsEnabled" . # �� HasDropShadow# .   TCStackPannelImageButton#��[ 6�   $��5q�6�   .+�5�   2   �� $��btnCall��69   -   5        " .   NormalImageButton#��[ 5�   �    5�   6   �� 5�   :   
����������#ADAAAD$	��0.5q�6j   $��-5q�6�   ._�5�   >   � 	 5�   B   �� 5�   F   $
��Auto �6T   6d   .+�5�   B   �� $��0��6j   $ 	10,5,10,5q�6w   $��Calibri)�6�   ��6L   �� *ImageToolTip+6�   5�   ?   5�   ;   .��5�   6   �� $��imgMessageIcon��6<   $��15��6R   $��15��6^   ��6i   �� *SearchIconImageSource+5�   7   5�   3   �� 5�   2   $��
btnContext��69   -   5        " .   TCArrowImageButton#��[ 5�   �   $��Bottom=�6�    5�   6   �� 5�   :   
����������#ADAAAD$	��0.5q�6j   $��-5q�6�   ._�5�   >   � 	 5�   B   �� 5�   F   $
��Auto �6T   6d   .+�5�   B   �� $��Action Menu��6L   $��0��6_   $ 	10,5,10,5q�6l   $��Calibri)�6�   6�   5�   ?   5�   ;   .��5�   6   �� $��imgicon��6<   $;��5/Agent.Interaction.Desktop;component/Images/Eject.png��6K   $��2,0,0,4q�6�   $��11��6�   $��11��6�   5�   ;   5�   7   5�   3   5�   +     5�   *   $ * 6D   ! 5�   .   �� 5�   2   .R�5�   6   �� $ Vertical=�6A   $��0,5,0,5q�6X   .+�5�   :   �� $��txtMessageSender��6r   $
��Left=�6D   
���   �$	��175��6�   $��2,0,0,0q�6�   ��6�   �� *SearchedName+ 5�   6   �� 5�   >   
����������#ADAAAD$	��0.5q�6n   $��-5q�6�   ._�5�   B   � 	 5�   F   �� 5�   J   $
��Auto �6X   6h   .+�5�   F   �� $��0��6n   $ 	10,5,10,5q�6{   $��Calibri)�6�   ��6P   �� *SearchedName+6�   5�   C   5�   ?   5�   3   �� 5�   :   $ 
Horizontal=�6E   .+�5�   >   �� $��imgicon��6D   $��15��6y   $��15��6�   ��6T   �� *StatusImageSource+6�   �� 5�   >   $��txtMessageSender1��6v   $
��Left=�6H   
���   �$��2,0,0,0q�6�   ��6�   �� *Status+ 5�   B   �� 5�   F   
����������#ADAAAD$	��0.5q�6v   $��-5q�6�   ._�5�   J   �� $ 
Horizontal=�6U   $��5q�6n   .+�5�   R   �� $��15��6Y   $��15��6e   ��6p   �� *MediaImageSource+�� 5�   R   $��Calibri)�6w   $��5,0,0,0q�6�   $��Center=�6�   ��6\   �� *	MediaType+6�   �� 5�   N   $��imgstatusicon��6T   $��20,0,0,0q�6�   $��10��6�   $��10��6�   ��6j   �� *StatusImageSource+6�   �� 5�   R   $��Calibri)�6t   $��Center=�6�   $��2,0,0,0q�6�   ��6\   �� *Status+6�   5�   K   5�      5�      5�      5�      5�      5�        5�      $ 25 6,   ! 5�      �� 5�      .R�5�      �� $��
imgFavicon��6$   $��15��6^   $��15��6i   ��67   �� *FavoriteImageSource+ 5�   "   �� 5�   &   
����������#ADAAAD$	��0.5q�6V   $��-5q�6l   ._�5�   *   � 	 5�   .   �� 5�   2   $
��Auto �6@   6P   .+�5�   .   �� $��0��6Y   $ 	10,5,10,5q�6f   $��Calibri)�6z   ��68   �� *FavoriteToolTip+6�   5�   +   5�   '   5�      5�      5�        5�      $ 45 6,   ! 5�      �� 5�      .R�5�      �� $��imgicon��6$   $��20��6Y   $��0,0,19,0q�6d   $��20��6v   ��64   �� *DetailImageSource+ 5�   6   �� 5�   :   
����������#ADAAAD$	��0.5q�6j   $��-5q�6�   ._�5�   >   � 	 5�   B   �� 5�   F   $
��Auto �6T   6d   .+�5�   B   �� $��0��6e   $ 	10,5,10,5q�6r   $��Calibri)�6�   ��6L   �� *	ToolTip+6�   5�   ?   5�   ;   5�   7   5�   +   5�   #   5�      �� 5�      $ Vertical=�6%   $
��Auto��6<   $��3��6J   .+�5�      �� $ 
Horizontal=�6)   .+�5�   "   �� $��0��6M   $��10,5,5,0q�6Z   $��25��6l   $��Calibri)�6x   ��6,   �� *InternalTargets+6�   5�      5�      5�      5�      �� 5�      -   5        $��txtErrorMessage��5�      $"��Stat Server is not available��63   $��Calibri)�6W   $��12(�6l   $��Center=�6z   $��Center=�6�   5      5     5     5     