   M S B A M L   `   `   `  ����  StatTickerFive7    1StatTickerFive.UserControls.View.TagGadgetControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -   5        $��TagUserControl��5        � 	Resources  5       �� MergedDictionaries �� Source�� 5	   
    5
      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�6%   5      %    5      .��5      �� -   5        $��
MainBorder��5   p   ��6   �� *
ThemeColor+��6/   �� *TagGadgetHeight+��6R   �� *GadgetWidth+._�5   
   � -   5        $��MainGrid��5       � ColumnDefinitions 5      �� 5      $��* �6#   6/   �� 5      $
��Auto �6#   62   �� 5      $
��Auto �6#   62    � RowDefinitions 5      �� 5      $
��Auto �6    60   �� 5      ��6    �� *GridStatObjectsHeight+6K   .+�5!      �� -   5        $��lblStatName��5!      $��0��6M   $
��Auto��6[   $��11(�6h   $��Segoe UI)�6v   $
��Auto��6�   $
��Left=�6�   $��Stretch=�6�   ��6+   �� *
StatWeight+��6�   �� *MainStatNameColor+
 � Tag 6�   �� *	StatRefId+  5'       �� 
TargetType �� BasedOn �� Property	 �� Value
 �� 
TargetType��%  '_�       7_��� 5(      " _�6   8   .�5)      ��  � ��$	 SemiBold"�67   �� 5*       � ��	 5+      �� 5,   "   "
 _�62   .R�5-   &   �� 
����������#ADAAAD$��1q�6V   ._�5.   *   �� #��z��6<   $��5q�6`   5/   '   50   #   52      53      54      55      �� 56      -   5        $��
lblStatObj��56      $��1��6+   $
��Auto��68   $��10(�6E   $��Segoe UI)�6S    � ToolTip$ Statistics Object��6i   $
��Auto��6�   $
��Left=�6�   $��Center=�6�   ��6�   �� *MainStatNameColor+ 6�   �� *	StatRefId+��6  �� *GridStatObjColspan+y�6;  �� *TagStatvaleVisible+  5>      ��%  '_�       7_��� 5?      " _�6   8   .�5@      ��  � ��$	 SemiBold"�67   �� 5A       � ��	 5B      �� 5C   "   "
 _�62   .R�5D   &   �� 
����������#ADAAAD$��1q�6V   ._�5E   *   �� #��z��6<   $��5q�6`   5F   '   5G   #   5I      5J      5K      5L      � 5O      -   5        $��GridStatValue��5O   #   $��1��6   ��69   �� *TagStatValueMargin+.+�5P      �� -   5        $��lblStatValue��5P      $ Statistics Value��6R   $��Right=�6m   $��Stretch=�6�   $��16(�6�   $
��Auto��6�   $��25��6�   $��Segoe UI)�6�   ��60   �� *
StatWeight+  5X      ��%  '_�       7_��� 5Y      " _�6    8   .�5Z      ��  � ��$	 SemiBold"�6;   �� 5[       � ��	 5\   "   �� 5]   &   "
 _�66   .R�5^   *   �� 
����������#ADAAAD$��1q�6Z   ._�5_   .   �� #��z��6@   $��5q�6d   5`   +   5a   '   5c      5d      5e      5f      5g      � 5i      -   5        $��GridUntagButton��5i      $��2��6*   $��22��6:   $��22��6E   y�6Q   �� *UnTagVisibility+.+�5j      �� 
������ $ Untag Statistics��62      NormalImageButton#��[  6M   $��21��6x   $��21��6�   $��Bottom=�6�   $��Center=�6�   ��6�   �� *UntagButtonCommand+��6�   ��  �� ElementName$ TagUserControl�� �� Path$	 Tag�.��5k      �� $��20��6   $��20��6'   $5��//StatTickerFive;component/Images/tag-remove.png��63   6o     5s      ��%  '_�       7_��� 5t      " _�6    8   .�5u      ��  � ��$	 SemiBold"�6;   �� 5v       � ��	 5w   "   �� 5x   &   "
 _�66   .R�5y   *   �� 
����������#ADAAAD$��1q�6Z   ._�5z   .   �� #��z��6@   $��5q�6d   5{   +   5|   '   5~      5      5�      5�      5�      5�      5�      5�      