   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	    StatTickerFive0   *StatTickerFive.UserControls.Views.TabValue   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   $
��Auto��5      $
��Auto��6-     � 	Resources  5       �� MergedDictionaries �� Source�� 5	   
    5
      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�6%   6g   %    5      .��5      � $	��658��6   $	��440��6    � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $��* �6   .+�5      � $��0��6   $��3,2,0,0q�6     � ColumnDefinitions 5      �� 5      $��25 �6'   64   �� 5      $��25 �6'   64   �� 5      $��25 �6'   64   .+�5      �� $��0��6   $��18��6%   $��22��61   $
��Left=�6<   $��1q�6W   
������    ImageButton#��[  6{   ��6�   �� *SaveObjectValuesCommand+.��5      �� $/��)/StatTickerFive;component/Images/save.png��6   $��16��6O   $��20��6[   6i    � ToolTip 5      �� 5      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5!      �� -   5        $��SaveObjects��5!   (   $��Save Changes��6;   $��0��6O    �� Padding$ 	10,5,10,0q�6\   $
��Bold"�6p   $��Calibri)�6�   6�   5#      5%      �� 5'      $��1��6   $��18��6)   $��22��65   $
��Left=�6@   $��1q�6[   
������ #��[  6�   ��6g   �� *UndoNewStatisticsCommand+.��5(      �� $/��)/StatTickerFive;component/Images/undo.png��6   $��16��6O   $��20��6[   6i    5)      �� 5*      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5,      �� -   5        $��UndoObjects��5,   $   $��Undo Changes��67   $��0��6K   $ 	10,5,10,0q�6X   $
��Bold"�6l   $��Calibri)�6~   6�   5.      50      �� 51      $��2��6   $��18��6)   $��22��65   $
��Left=�6@   $��1q�6[   
������ #��[  6�   ��6g   �� *EditObjectsCommand+.��52      �� $8��2/StatTickerFive;component/Images/ObjectSetting.png��6   $��16��6T   $��20��6`   6n    53      �� 54      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�56      �� -   5        $��ObjectSettings��56   $   $��Object Settings��6:   $��0��6Q   $ 	10,5,10,0q�6^   $
��Bold"�6r   $��Calibri)�6�   6�   58      5:      5<      �� 5=   
   $	��0.5q�69   $��1��6O   $
��Auto��6\   $
��Auto��6i   $��2q�6w   ��6   �� *TitleBackground+._�5>      �  5?      �� 5@      $��25 �6$   62   �� 5A      $��* �6$   61    5C      �� 5D      $	��220 �6'   65   �� 5E      $��* �6'   63   .+�5G      �� $��0��6   $��0��6&   $��1q�66   ���#D6D7D6
�������6`   �� 5H      $��0��6   $��0��6%   $��Section Name��65   $��Center=�6L   $
��Bold"�6g   $��Center=�6y   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5I      $��0��6   $��1��6&   $��1q�66   ���#D6D7D6
�������6`   �� 5J      $��0��6   $��1��6%   $��Description��65   $��Center=�6K   $
��Bold"�6f   $��Center=�6x   $��12(�6�   $��Calibri)�6�   
���   �6�   �� 5K      $	��0.5q�6A   $��1��6W   $��0,-2,0,0q�6d   $��0��6w   $��2��6�   $	��435��6�   $	��606��6�   ��6   �� *TitleBackground+._�5L      �� $��1��6#   $��0��60   $��2��6@   $	��433��6T   $	��606��6`   .��5M      � .+�5N      &   System.Windows.Controls.DataGrid  -   5        $��ExistingStatistics��5N   E      CanUserReorderColumns .     HorizontalGridLinesBrush �#D6D7D6	   VerticalGridLinesBrush	 �#D6D7D6
�������
   CanUserResizeColumns
 .     CanUserResizeRows .     HeadersVisibility$
 None=�6�      AutoGenerateColumns .     CanUserAddRows .    
CustomGrid#��[ 6�  $	��440��6�  $   HorizontalScrollBarVisibility$ Disabled=�6�  "   VerticalScrollBarVisibility$ Disabled=�6	     
IsReadOnly .  ��6  �� *ObjectStatistics+ �� NotifyOnTargetUpdated .  �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�   Columns 5O   "   4  .System.Windows.Controls.DataGridTemplateColumn  5P   &   ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 25 6=      CellTemplate 5Q   *   �� 5R   .   .R�5S   2   �� $��chkbox��6;   $��5q�5T   L   $��20��6  ��5S   I   �� *DataContext.ObjectSelected+ �� RelativeSource ��  �� AncestorType$ DataGridL���5T   /   ��  d� 	IsChecked 6W   �� *isGridChecked+$ TwoWay=�$ PropertyChanged=�{�6�   �� *IsCheckBoxEnabled+$ TwoWay=�$ PropertyChanged=�5U   3   5V   /   5X   '     5Z   &   $	 194 6=      Header$ Section/Statistics Name��6I    5[   *   �� 5\   .   .R�5]   2   �� $��lblStatName��6<   $ 5,0,0,0q�6O   ��6a   �� *
ObjectName+$ TwoWay=�$ PropertyChanged=� 5^   6   �� 5_      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5a   >   �� $��Section��6H   $��0��6�   $ 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6W   �� *ObjectsNameTT+$ TwoWay=�$ PropertyChanged=�6�   5c      5e   3   5f   /   5h   '     5j   &   $	 195 6=   $ Description��6I    5k   *   �� 5l   .   .R�5m   2   �� $��lblDisplayName��6<   $ 5,0,0,0q�6R   ��6d   �� *ObjectDescription+$ TwoWay=�$ PropertyChanged=� 5n   7   �� 5o      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5q   >   �� $��Description��6H   $��0��6�   $ 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6[   �� *DescriptionTT+$ TwoWay=�$ PropertyChanged=�6�   5s      5u   3   5v   /   5x   '   5{        5|      -   5        $��AvailStatistics��5|   E    .   �#D6D7D6	 �#D6D7D6
�������
 .   .  $
 None=�6�    .   .  #��[ 67  $	��440��6[  $
��Auto��6g  $ Disabled=�6u  $ Disabled=�6�   .  ��6�  �� *NewStatistics+ . $ PropertyChanged=�$ TwoWay=� 5}   "     5~   &   $ 25 6=    5   *   �� 5�   .   .R�5�   2   �� $��chkbox��6;   $��5q�6I   $��20��6T    6_   �� *isGridChecked+$ TwoWay=�$ PropertyChanged=���5�   7   �� *!DataContext.SelectNewStatistics+ �� $ DataGridL���6�   �� 5�   3   5�   /   5�   '     5�   &   $	 194 6=   $ Section/Statistics Name��6I    5�   *   �� 5�   .   .R�5�   2   �� $��lblStatName��6<   $ 5,0,0,0q�6O   ��6a   �� *
ObjectName+$ TwoWay=�$ PropertyChanged=� 5�   6   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�   >   �� $��SectionNameAvail��6H   $��0��6�   $ 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6`   �� *ObjectsNameTT+$ TwoWay=�$ PropertyChanged=�6�   5�      5�   3   5�   /   5�   '     5�   &   $	 195 6=   $ Description��6I    5�   *   �� 5�   .   .R�5�   2   �� $��lblDisplayName��6<   $ 5,0,0,0q�6R   ��6d   �� *ObjectDescription+$ TwoWay=�$ PropertyChanged=� 5�   6   �� 5�      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5�   >   �� $��DescriptionAvail��6H   $��0��6�   $ 	10,5,10,0q�6�   $
��Bold"�6�   $��Calibri)�6�   ��6`   �� *DescriptionTT+$ TwoWay=�$ PropertyChanged=�6�   5�      5�   3   5�   /   5�   '   5�      5�      5�      5�      5�      5�      5�      5�      