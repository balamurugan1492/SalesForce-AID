   M S B A M L   `   `   `  ����)  %Pointel.Interactions.TeamCommunicatorI    CPointel.Interactions.TeamCommunicator.UserControls.TeamCommunicator   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      
�������  � 	Resources  5       �� MergedDictionaries �� Source   GroupHeaderStyle �� 
TargetType �� Property �� Value �� 
TargetType 9�Name 9� 
IsExpanded�� 5	   
    5
      �� 5      $^ X/Pointel.Interactions.TeamCommunicator;component/Resources/StylesResourceDictionary.xaml?�6%   %Q  &	        �� 5      " ��6-   .�5      ��  � �� 5      �� 5      " ��6*   .R�5      9� $	 exp��6'    . ���#007edf
���������5   "   �� 5   &   $��Calibri)�6F   $
��Bold"�6[   $��Normal&�6n   
���������60   �� *Name+5   '   .��5   "   �� 5      5      5      5      5       .��5"      � $
��Auto��6   .+�5#   
   �� ���#007edf._�5$      � $	��210��6   	 � RowDefinitions	 5%      �� 5&      $
��Auto �6$   64   �� 5'      $��* �6$   61   .+�5)      �� -   5        $��	txtSearch��5)   )   
 �� 	UndoLimit$
 10��6   $��5,5,5,0q�6:   ���#007edf$��3q�6a   $��0��6m   $	��125��6z   $��Stretch=�6�   $	��Top=�5*      $��23��62   6Z   � 5+      -   5        $��
grdContent��5+   ~   $��5,0,5,5q�6   
�������$��1��6;   $��Stretch=�6H   $	��Top=�6f   	 5,      �� 5-      -   5         �Name$ rowMenus��5-   (   $
��Auto �68   6H   �� 5.      -   5        $ 
rowContent��5.   (   $��0 �6:   6G   �� 5/      -   5        $ 	rowfooter��5/   (   $
��Auto �69   6I   .+�51      �� ���#007edf$��0,3,0,5q�63   $��0��6D   ._�52      �� -   5        $��grdMenus��52   %    �� Orientation$ 
Horizontal=�65   $��Stretch=�6N   $
��Auto��6l   $
��Auto��53   !   $	��Top=�6/   .+�54      d� -   5        $��btnSelectAll��54   '   $��2q�6;   $��Right=�6F     
SideButton#��[ 6b   .��55      �� $;��5/Agent.Interaction.Desktop;component/Images/Agent.png��6$   $��15��6c   $��20��6n   56      d� 57      -   5        $��btnFavorite��57   '   $��2q�6:   $��Right=�6E   #��[ 6a   .��58      �� $;��5/Agent.Interaction.Desktop;component/Images/Agent.png��6$   $��15��6c   $��20��6n   59      d� 5:      -	   5        $��	btnRecent��5:   '   $��2q�68   $��Right=�6C   #��[ 6_   .��5;      �� $;��5/Agent.Interaction.Desktop;component/Images/Agent.png��6$   $��15��6c   $��20��6n   5<      �� 5=      -
   5        $��	cmbFilter��5=   #   $��2q�64   $��Right=�6?   $	��105��6[   $��23��6g   5?      d� 5@      -   5        $��btnGroup��5@   '   $��2q�67   $��Right=�6B   #��[ 6^   .��5A      �� $;��5/Agent.Interaction.Desktop;component/Images/Agent.png��6$   $��15��6c   $��20��6n   5B      5C      5D      )  #Microsoft.Windows.Controls.DataGrid  5E      -   5        $��DGTeamCommunicator��5G      $��Stretch=�5E   "   $   HorizontalScrollBarVisibility$ Disabled=�6>   "   VerticalScrollBarVisibility$
 Auto=�6g      RowHeaderWidth$ 0Q�6�   $��5q�6�   
�������$��Stretch=�5F   1   
�������$
��Left=�5G   /      HorizontalGridLinesBrush �#D6D7D6   GridLinesVisibility$
 None=�6t      VerticalGridLinesBrush �#D6D7D6   AutoGenerateColumns .     CanUserAddRows .     CanUserDeleteRows .     CanUserSortColumns .  $��1��5I   H      HeadersVisibility$
 None=�6U   ��60   �� 5K      �� 5L      $ 
Horizontal=�6!   $��2��6:   $y�	Collapsed=�6G   .+�5M      �� -   5        $��txtMatchingResults��5M   $   $
��Left=�6>   $
��Auto��6Y   $��10,5,5,0q�6f   $��25��6x   5N      5O      5P      5Q      5R      5S      