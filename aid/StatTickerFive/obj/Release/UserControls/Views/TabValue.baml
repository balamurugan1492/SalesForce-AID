   M S B A M L   `   `   `  ���� c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity   StatTickerFive0   *StatTickerFive.UserControls.Views.TabValue   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     $
��Auto��$
��Auto��  � 	Resources   �� MergedDictionaries �� Source��  �� $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�%    .��� $	��658��$	��440�� � RowDefinitions �� $��20 ��� $��* �.+�� $��0��$��3,2,0,0q� � ColumnDefinitions �� $��25 ��� $��25 ��� $��25 �.+��� $��0��$��18��$��22��$
��Left=�$��1q�
������    ImageButton#��[  ���� *SaveObjectValuesCommand+.���� $/��)/StatTickerFive;component/Images/save.png��$��16��$��20�� � ToolTip �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��SaveObjects��$��Save Changes��$��0�� �� Padding$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��1��$��18��$��22��$
��Left=�$��1q�
������ #��[  ���� *UndoNewStatisticsCommand+.���� $/��)/StatTickerFive;component/Images/undo.png��$��16��$��20�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��UndoObjects��$��Undo Changes��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $��2��$��18��$��22��$
��Left=�$��1q�
������ #��[  ���� *EditObjectsCommand+.���� $8��2/StatTickerFive;component/Images/ObjectSetting.png��$��16��$��20�� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� -   $��ObjectSettings��$��Object Settings��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)��� $	��0.5q�$��1��$
��Auto��$
��Auto��$��2q����� *TitleBackground+._��  �� $��25 ��� $��* � �� $	��220 ��� $��* �.+��� $��0��$��0��$��1q����#D6D7D6
��������� $��0��$��0��$��Section Name��$��Center=�$
��Bold"�$��Center=�$��12(�$��Calibri)�
���   ��� $��0��$��1��$��1q����#D6D7D6
��������� $��0��$��1��$��Description��$��Center=�$
��Bold"�$��Center=�$��12(�$��Calibri)�
���   ��� $	��0.5q�$��1��$��0,-2,0,0q�$��0��$��2��$	��435��$	��606������ *TitleBackground+._��� $��1��$��0��$��2��$	��433��$	��606��.��� .+�&   System.Windows.Controls.DataGrid  -   $��ExistingStatistics��   CanUserReorderColumns .     HorizontalGridLinesBrush �#D6D7D6	   VerticalGridLinesBrush	 �#D6D7D6
�������
   CanUserResizeColumns
 .     CanUserResizeRows .     HeadersVisibility$
 None=�   AutoGenerateColumns .     CanUserAddRows .    
CustomGrid#��[ $	��440��$   HorizontalScrollBarVisibility$ Disabled=�"   VerticalScrollBarVisibility$ Disabled=�   
IsReadOnly .  ���� *ObjectStatistics+ �� NotifyOnTargetUpdated .  �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 25    CellTemplate �� .R��� $��chkbox��$��5q�$��20������ *DataContext.ObjectSelected+ �� RelativeSource ��  �� AncestorType$ DataGridL�����  d� 	IsChecked �� *isGridChecked+$ TwoWay=�$ PropertyChanged=�{��� *IsCheckBoxEnabled+$ TwoWay=�$ PropertyChanged=�  $	 194    Header$ Section/Statistics Name�� �� .R��� $��lblStatName��$ 5,0,0,0q����� *
ObjectName+$ TwoWay=�$ PropertyChanged=� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��Section��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *ObjectsNameTT+$ TwoWay=�$ PropertyChanged=�  $	 195 $ Description�� �� .R��� $��lblDisplayName��$ 5,0,0,0q����� *ObjectDescription+$ TwoWay=�$ PropertyChanged=� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��Description��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *DescriptionTT+$ TwoWay=�$ PropertyChanged=�  -   $��AvailStatistics�� .   �#D6D7D6	 �#D6D7D6
�������
 .   .  $
 None=� .   .  #��[ $	��440��$
��Auto��$ Disabled=�$ Disabled=� .  ���� *NewStatistics+ . $ PropertyChanged=�$ TwoWay=�   $ 25  �� .R��� $��chkbox��$��5q�$��20�� �� *isGridChecked+$ TwoWay=�$ PropertyChanged=����� *!DataContext.SelectNewStatistics+ �� $ DataGridL�����   $	 194 $ Section/Statistics Name�� �� .R��� $��lblStatName��$ 5,0,0,0q����� *
ObjectName+$ TwoWay=�$ PropertyChanged=� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��SectionNameAvail��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *ObjectsNameTT+$ TwoWay=�$ PropertyChanged=�  $	 195 $ Description�� �� .R��� $��lblDisplayName��$ 5,0,0,0q����� *ObjectDescription+$ TwoWay=�$ PropertyChanged=� �� 
����������#ADAAAD$	��0.5q�$��-5q�._��� $��DescriptionAvail��$��0��$ 	10,5,10,0q�$
��Bold"�$��Calibri)����� *DescriptionTT+$ TwoWay=�$ PropertyChanged=�