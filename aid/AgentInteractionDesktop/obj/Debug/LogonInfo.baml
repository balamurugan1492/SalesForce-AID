   M S B A M L   `   `   `  ����U  QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls  5   	   )    #Agent.Interaction.Desktop.LogonInfo   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   PCustomControl6clr-namespace:Agent.Interaction.Desktop.CustomControls     5   	   -   5          � Title$  	LogonInfo��5   	    � AllowsTransparency .  � WindowStyle$
 None=�65    � 
ResizeMode$ NoResize=�6H    � WindowStartupLocation$ CenterScreen=�6_    � ShowInTaskbar .   � Topmost . $
��Auto��5   
   $	��320��6   ���#656d90$��1q�6:    � SizeToContent$ Height=�6N    � Icon$F @/Agent.Interaction.Desktop;component/Images/pointelicon16x16.ico��5   	   	 � 	Resources	 5      
 �� MergedDictionaries �� Source�� 5	   
   
 5
      �� 5      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   .+�5   
   �� -   5        $��lblTitle��5      $��0��6   $��14(�6-   $��Stretch=�6;   $��Calibri)�6Y   ���#007edf
�������$��Log On Information��5   V   6u   �� 5   
   $��1��6   $��2��6   ���#656d90$	��0.5q�6E   $��5q�6[   �� 5   
   -   5        $��gbVoiceChannel��5   <   $��10q�6   $��1��6    $��50��6-   ���#ADAAAD$	��0.5q�6h   ��5      �� 5      -   5         ��Name$ 
chkbxVoice��5         KeyVoice#���   6/   $��15��6T   {�.  d� 	IsChecked 6`   �� *IsVoiceChecked+5      .��5      � $��2q�6   $
��Auto��6    5      �� 5       ��6$   �� *LoginQueueRowHeight+6M   �� 5!      ��6$   �� *AgentLoginIDRowHeight+6O   �� 5"      ��6$   �� *AgentPasswordRowHeight+6P    � ColumnDefinitions 5$      �� 5%      $
��158* �6'   66   �� 5&      $
��234* �6'   66   .+�5(      �� -   5        $��lblQueue��5(        KeyQueue#���  6(   $��0��6M   $��0��6Z   {�6j   ��  �� ElementName$ 
chkbxVoice�� �� Path$ CheckBox.IsChecked��� 5)      -   5        $��cbQueue��5)   B   $��23��6   $
��Left=�6'   $	��Top=�6Q   $��1��5*   a    �� 
IsEditable . $��1,0,0,0q�6�   ��5)   i   �� *LoadCollection+ �� SelectedValue 5*   '   �� *QueueSelectedValue+ �� Mode$ TwoWay=�{�6�   �� $ 
chkbxVoice��$ CheckBox.IsChecked�6�   �� 5+      -   5        $��lblAgentLogin��5+      $��Center=�6-     KeyAgentLogin#���  6O   $��1��6y   $��0��6�   {�6�   �� $ 
chkbxVoice��$ CheckBox.IsChecked�6�   �� 5,      -   5        $��lblAgentLoginID��5,      $�� ��6/   $��1��6;   $��1��6H   $��2��6�   {�6X   �� $ 
chkbxVoice��$ CheckBox.IsChecked�6�   �� 5-      -	   5        $��lblAgentPassword��5-      $��Center=�60     KeyAgentPassword#���  6R   $��2��6   $��0��6�   {�6�   �� $ 
chkbxVoice��$ CheckBox.IsChecked�6�   G� 5.      -
   5        $��txtAgentPassword��5.      $��2��66   $��1��6C   $��Stretch=�6S   $��2��6�     controltempPasswordbox#��[ 6�   $��2q�6�   {�6q   �� $ 
chkbxVoice��$ CheckBox.IsChecked�6(  5/      50      �� 51   
   -   5        $��btnOk��51      $��2��6   $��0,10,105,10q�6+   $��Right=�6@   $��Apply��6\   $��23��6l   $��80��6x     NormalButton#��[ 6�   6�   �� 53   
   -   5        $��	btnCancel��53      $��2��6"   $��
0,10,10,10q�6/   $��Right=�6C   $��Cancel��6_   $��23��6p   $��80��6|   #��[ 6�   6�   54      55      