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
 � Tag 6�   �� *	StatRefId+ � ToolTip 5"      �� 5#      
����������#ADAAAD$	��0.5q�6F   $��-5q�6\   ._�5%      �� -   5        $��StatTooltip��5%   $   $��0��67    �� Padding$ 	10,5,10,0q�6D   $
��Bold"�6X   $��Calibri)�6j   6�   5'      5)      �� 5*      -   5        $��
lblStatObj��5*      $��1��6+   $
��Auto��68   $��10(�6E   $��Segoe UI)�6S   $
��Auto��6�   $
��Left=�6�   $��Center=�6�   ��6i   �� *MainStatNameColor+ 6�   �� *	StatRefId+��6�   �� *GridStatObjColspan+y�6  �� *TagStatvaleVisible+6K  � 5-      -   5        $��GridStatValue��5-   #   $��1��6   ��69   �� *TagStatValueMargin+.+�5.      �� -   5        $��lblStatValue��5.      $��Right=�6R   $��Stretch=�6n   $��16(�6�   $
��Auto��6�   $��25��6�   $��Segoe UI)�6�   ��60   �� *
StatWeight+6�   5/      � 51      -	   5        $��GridUntagButton��51      $��2��6*   $��22��6:   $��22��6E   y�6Q   �� *UnTagVisibility+.+�52      �� 
������    NormalImageButton#��[  62   $��21��6]   $��21��6i   $��Bottom=�6u   $��Center=�6�   ��6�   �� *UntagButtonCommand+��6�   ��  �� ElementName$ TagUserControl��	 �� Path$		 Tag�.��53      �� $��20��6   $��20��6'   $5��//StatTickerFive;component/Images/tag-remove.png��63   6o    54      �� 55      
����������#ADAAAD$	��0.5q�6J   $��-5q�6`   ._�57      �� $��Untag Statistics��6(   $��0��6@   $ 	10,5,10,0q�6M   $
��Bold"�6a   $��Calibri)�6s   6�   59      5;      5<      5=      5>      5?      