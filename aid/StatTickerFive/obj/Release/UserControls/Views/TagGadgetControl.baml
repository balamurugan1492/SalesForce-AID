   M S B A M L   `   `   `  ����   StatTickerFive7    1StatTickerFive.UserControls.View.TagGadgetControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -   $��TagUserControl��  � 	Resources   �� MergedDictionaries �� Source��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%    .���� -   $��
MainBorder������ *
ThemeColor+���� *TagGadgetHeight+���� *GadgetWidth+._�� -   $��MainGrid�� � ColumnDefinitions �� $��* ��� $
��Auto ��� $
��Auto � � RowDefinitions �� $
��Auto ��� ���� *GridStatObjectsHeight+.+��� -   $��lblStatName��$��0��$
��Auto��$��11(�$��Segoe UI)�$
��Auto��$
��Left=�$��Stretch=����� *
StatWeight+���� *MainStatNameColor+
 � Tag �� *	StatRefId+   �� 
TargetType �� BasedOn �� Property	 �� Value
 �� 
TargetType��%�   '_�       7_��� " _�8   .���  � ��$	 SemiBold"���  � ��	 �� "
 _�.R��� 
����������#ADAAAD$��1q�._��� #��z��$��5q��� -   $��
lblStatObj��$��1��$
��Auto��$��10(�$��Segoe UI)� � ToolTip$ Statistics Object��$
��Auto��$
��Left=�$��Center=����� *MainStatNameColor+ �� *	StatRefId+���� *GridStatObjColspan+y��� *TagStatvaleVisible+  ��%�   '_�       7_��� " _�8   .���  � ��$	 SemiBold"���  � ��	 �� "
 _�.R��� 
����������#ADAAAD$��1q�._��� #��z��$��5q�� -   $��GridStatValue��$��1������ *TagStatValueMargin+.+��� -   $��lblStatValue��$ Statistics Value��$��Right=�$��Stretch=�$��16(�$
��Auto��$��25��$��Segoe UI)����� *
StatWeight+  ��%�   '_�       7_��� " _�8   .���  � ��$	 SemiBold"���  � ��	 �� "
 _�.R��� 
����������#ADAAAD$��1q�._��� #��z��$��5q�� -   $��GridUntagButton��$��2��$��22��$��22��y��� *UnTagVisibility+.+��� 
������ $ Untag Statistics��   NormalImageButton#��[  $��21��$��21��$��Bottom=�$��Center=����� *UntagButtonCommand+����  �� ElementName$ TagUserControl�� �� Path$	 Tag�.���� $��20��$��20��$5��//StatTickerFive;component/Images/tag-remove.png��  ��%�   '_�       7_��� " _�8   .���  � ��$	 SemiBold"���  � ��	 �� "
 _�.R��� 
����������#ADAAAD$��1q�._��� #��z��$��5q�