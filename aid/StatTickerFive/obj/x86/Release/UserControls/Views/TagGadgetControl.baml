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
 � Tag �� *	StatRefId+ � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��StatTooltip��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� -   $��
lblStatObj��$��1��$
��Auto��$��10(�$��Segoe UI)�$
��Auto��$
��Left=�$��Center=����� *MainStatNameColor+ �� *	StatRefId+���� *GridStatObjColspan+y��� *TagStatvaleVisible+� -   $��GridStatValue��$��1������ *TagStatValueMargin+.+��� -   $��lblStatValue��$��Right=�$��Stretch=�$��16(�$
��Auto��$��25��$��Segoe UI)����� *
StatWeight+� -	   $��GridUntagButton��$��2��$��22��$��22��y��� *UnTagVisibility+.+��� 
������    NormalImageButton#��[  $��21��$��21��$��Bottom=�$��Center=����� *UntagButtonCommand+����  �� ElementName$ TagUserControl��	 �� Path$		 Tag�.���� $��20��$��20��$5��//StatTickerFive;component/Images/tag-remove.png�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��Untag Statistics��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)�