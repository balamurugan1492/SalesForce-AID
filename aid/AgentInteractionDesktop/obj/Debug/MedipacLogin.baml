   M S B A M L   `   `   `  ����  Agent.Interaction.Desktop,    &Agent.Interaction.Desktop.MedipacLogin   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  MedipacLogin��5   	   $	��400��6   $
��Auto��6*    � WindowStyle$
 None=�68    � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��6K    � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�5   6    � SizeToContent$ Height=�6[    � 	Resources 5       �� MergedDictionaries �� Source   CantarellFontFamily	 �� 
TargetType
 �� Property �� Value�� 5   
    5	      �� 5
      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %�   &	        '� O     *�5      ;7/Agent.Interaction.Desktop;component/Resources/#Calibri)�6o   �� 5      "	 �6   .�5      �� 
 � ��$ Calibri)�6/   5      5      .��5      � .+�5   
   �� -   5        $��
MainBorder��5   S   $��0��6   $
��0.89q�6   $��8q�65   
�������._�5      �  � RowDefinitions 5      �� 5      -   5         �Name$ mainRowHeader��5   $   $��50 �69   6G   �� 5      -   5        $ mainRowError��5   $   6:   �� 5      -   5        $ mainRowContent��5   %   $
��Auto �6;   6K   �� 5      -   5        $ mainRowFooter��5   $   6;   .+�5      � -   5        $��0��5      
������� � ColumnDefinitions 5      �� 5       $
��Auto �6+   �� 5!      $
��Auto �6+   .+�5#      �� -   5        $��image1��5#      $��0��6*   $��Right=�6:   $��55,3,0,5q�6V   $
��Auto��6h   $��40��6u   $G��A/Agent.Interaction.Desktop;component/Images/Pointel Softphone.png��6�   �� 5$      -	   5        $��10,0,0,0q�5$   !   $��1��63   $
��Left=�6C    �� Orientation$ Vertical=�6^   .+�5%      �� -
   5        $��
loginTitle��5%       $��18(�6\   $��0,3,0,0q�6j   ���#007edf$
��Bold"�6�     keyTitle#���  6�   6�   �� 5&      -   5        $��loginTitleversion��5&       $��
140,-7,0,3q�6c     keyTitleVersion#���  6w   $��12(�6�   6�   5'      5)      �� 5*      -   5        $��
stkp_Error��5*   O   $ 
Horizontal=�6   $��3q�66   $��1��6B   
��� ���$��Stretch=�6u   .+�5+      �� $��5,2,0,2q�6   $	��Top=�6-   $;��5/Agent.Interaction.Desktop;component/Images/Error.png��6E   $��20��6�   $��20��6�   �� 5,      -   5        $��lblError��5,       $��10,5,5,5q�60   $	��320��6B   $
��Wrap=�6N   5-      5.      �� 5/      $��2��6   $ Vertical=�6+   $��Stretch=�6B   $��5q�6a   .+�50      �� $��Info Net Login��6   ���#ADAAAD$��1q�6M   .��51      � $��5q�6    52      �� 53   "   $��28 �60   6>   �� 54   "   $��28 �60   6>    56      �� 57   "   $
��Auto �63   $	��120��6@   6Q   �� 58   "   $��* �63   6?   .+�5:      �� -   5        $��lblInfoNetUsername��5:   $   $��	User Name��6>   6T   �� 5;      -   5        $��txtInfoNetUserName��5;   E   $��1��6&    �� 	UndoLimit$ 10��66     DefaultCopyContextmenu � ContextMenu# [ 6`    �� SelectedText$  ��5<      
 � Tag$ vinoth��6   $��Stretch=�6)   $��2q�6G     controltempTextbox#��[ 5=      -  'System.Windows.Input.KeyboardNavigation   TabIndex$ 0��6;      TabNavigation$ Local=�5>   *    �� 	MaxLength$	 255��6S   �� 5?      -   5        $��lblInfoNetPassword��5?   $   $��Password��6>   $��0��6Q   $��1��6a   6p   G� 5A      -   5        $��txtInfoNetPassword��5A   *   $��1��6D   $��2��6Q   $��Stretch=�6a   $��2q�6     controltempPasswordbox#��[ 5B   %   $ 1��6X   $ Local=�5C   %   # [ 6O   6�   5E      5G      �� 5H      -   5        $��	gbMedipac��5H      $��MediPac��60   ���#ADAAAD$��1q�6W   $��0,5,0,0q�6k   .��5I      � $��5q�6    5J      �� 5K   "   $��28 �60   6>   �� 5L   "   $��28 �60   6>   �� 5M   "   $��28 �60   6>    5O      �� 5P   "   $
��Auto �63   $	��120��6@   6Q   �� 5Q   "   $��* �63   6?   .+�5S      �� -   5        $��lblMediPacUsername��5S   $   $��	User Name��6>   6T   �� 5T      -   5        $��txtMediPacUserName��5T   E   $��1��6&   $ 10��66   # [ 6`   $  ��5U      $ vinoth��6   $��Stretch=�6)   $��2q�6G   #��[ 5V      $ 0��5W      $ Local=�6-   $	 255��6V   �� 5X      -   5        $��lblMediPacPassword��5X   $   $��Password��6>   $��0��6Q   $��1��6a   6p   G� 5Z      -   5        $��txtMediPacPassword��5Z   *   $��1��6D   $��2��6Q   $��Stretch=�6a   $��2q�6   #��[ 5[   %   $ 1��6Y   $ Local=�5\   %   # [ 6O   6�   �� 5]      -   5        $��cbxKeepPlace��5]   '   $��2��6<   $��2��6I   
���  ��$��5q�6p   $��23��6{   $��Center=�5^   #   $ 6��6E   $ Local=�6e   $�� Same as Info Net��6�   6�   5a      5b      �� 5c      $��Right=�6!   $ 
Horizontal=�6=   .+�5d      �� -   5        $��btnOk��5d   .   $��23��6!   $��5,5,5,0q�6<   $��Center=�6M   $��70��6h     NormalButton#��[ 5e   %   $��Ok��6_   �� 5f      -   5        $��	btnCancel��5f   -   $��23��6!   $��70��6>   $��5,5,0,0q�6J   $��Center=�6\   #��[ 5g   %   $��Cancel��6c   5i      5j      �� 5k      $��31��6   ���#007edf$��Stretch=�6:   $	��Top=�6X   $��3��6p   $��0q�6}   ._�5l      � .+�5m      �� $��© 2013-2016 Pointel Inc��6$   $��Calibri)�6C   $
��Left=�6Y   $��Center=�6t   $��12(�6�   
�������$��5,0,0,0q�6�   5n      5o      5p      5q      5r      5t      