   M S B A M L   `   `   `  ����  PedagogyPal"    PedagogyPal.Windows.HomePage   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  PedagogyPal��5   	   $	��900��6   $
��1600��6*    � WindowStartupLocation$ CenterScreen=�5   	      BackgroundBrush#���   5   	    � 	Resources 5	        SlideInSidebar �� 
TargetName �� TargetProperty W� From	 W� To g� Duration  SlideOutSidebar��%H  &	       &	 �     �� 5   
   .��5      W� $ SidebarTranslateTransform��5      $ X�5      $
 -300X�5      $ 0X�6   $ 0:0:0.3D�5      5      �� 5   
   .��5      W� $ SidebarTranslateTransform��5      $ X�5      $ 0X�5      $
 -300X�6   $ 0:0:0.3D�5      5      5      .��5      � .+�5   
   � .+�5!      ]�   PrimaryBrush#���  6   $��94��6D   $	��Top=�6P   .+�5#      �� -   5         ��Name$ HamburgerButton��5#      $	��☰��62     HamburgerButtonStyle#��[ 6>   $��70��5$      $��72��6$   $��10q�60   $
��Left=�5%   7   �� 5(      $��PedagogyPal��6   $��32(�6/   $
��Bold"�6=   
�������$��Center=�5)      $��10,0,0,0q�67     
HeaderFont#��[ 5*      5+      � 5.      $��30,100,30,30q�6   	 � ColumnDefinitions	 5/      �� 50      $��3* �6'   �� 51      $��2* �6'   
 � RowDefinitions
 53      �� 54      $
��Auto �6$   �� 55      $��* �6$   .+�59      �� $��0��6   $��0��6)   $��2��66   
������� �� CornerRadius$ 10��5;      #���  5<      $��1q�5=       �� Padding$ 20q�5>        DropShadowEffect J� Effect# [ 5?      $��
0,22,404,0q�6D   ._�5@      �� .+�5B      �� -   5        $��Today��5B   !     RaisedButtonStyle#��[ 61   $	��140��5C   !   $��0,0,0,20q�6G   $
��Left=�6Y   &   System.Windows.Controls.Calendar  5F      -   5          Name$ MainCalendar��5F   #     CustomCalendarStyle#��[ 5I   #      SelectedDate 5G   #   �� *SelectedDate+ �� Mode$ TwoWay=�   DisplayDate 5H   #   �� *DisplayDate+$ TwoWay=�5I   #   5K      5L      �� 5O      $��0��6   
�������$ 10��5Q      #���  5R      $��1q�5S      $ 20q�5T      # [ 5U      $��2��6D   $��
540,24,0,1q�6X   $��2��6l   ._�5V      �� .+�5W      �� $��
Your Tasks��6$   $��26(�66   $
��Bold"�6D   #���  5X   $   $��0,0,0,15q�6P   #��[ 5Y   $   �� 5\       �� Orientation$ 
Horizontal=�6%   $��0,0,0,15q�6>   .+�5]      ��  	 SearchBoxStyle#��[	 5^   &   $	��400��5_   &   $��45��62   $��Center=�5`   &   ��5]   &   �� *SearchQuery+ �� UpdateSourceTrigger$ PropertyChanged=�5`   &   �� 5a      $��Clear��6%    
 OutlinedButtonStyle#��[
 65   $	��120��5c   %   $��45��61   $��15,0,0,0q�6=   ��5b   %   �� *ClearSearchCommand+5c   =   �� 5d      $��Show All��6%   #��[
 68   $	��120��5f   %   $��45��61   $��15,0,0,0q�6=   ��5e   %   �� *ClearFilterCommand+5f   =   5g      �� 5j        StandardListViewStyle#��[ 5l        StandardListViewItemStyle#��[ 5m      $
��Auto=�5n      $	��482��6O   ��5j   #   �� *Tasks+ �� SelectedItem 5k      �� *SelectedTask+ �� View 5o      �� 5p   "   .P�5q   &   �� $��Title��65    �� Width$	 200��6k    �� DisplayMemberBinding 6D   �� *Title+6k   �� 5r   &   $��Description��65   $	 300��6w    6J   �� *Description+6w   �� 5s   &   $��Due Date��65   $	 150��6�    6G   �� *	DueDate+ �� StringFormat$ {0:MM/dd/yyyy}��6�   5t   #   5v      �� 5y      $ 
Horizontal=�6%   $
��Left=�6>   $��0,15,0,0q�6Y   .+�5z      �� $��Add Task��6%   #��[ 68   $��0,0,15,0q�5{   4   $	��140��6F   ��6   �� *AddTaskCommand+6F   �� 5|      $��	Edit Task��6%   #��[
 69   $��0,0,15,0q�5}   5   $	��140��6G   ��6   �� *EditTaskCommand+6G   �� 5~      $��Delete Task��6%   #��[
 6;   $	��140��5   7   ��6   �� *DeleteTaskCommand+67   5�      5�      5�      5�      5�      � 5�   
   -   5         �Name$ Sidebar��5�      #���  6     K� ZIndex$ 10��5�      $	��300��5�      $
��Left=�5�      $��Stretch=�6*   z�5�      T� 5�      -   5        SidebarTranslateTransform�� T� X$
 -300Q�5�   H   .+�5�      �� $ 	0,0,10,10��6    5�      3  -System.Windows.Media.Effects.DropShadowEffect  5�         Color$ Black��6'      
BlurRadius$ 10Q�65      ShadowDepth$ 5Q�6E      Opacity$	 0.5Q�6U   ._�5�      �� $��0,80,0,0q�6   .+�5�      �� $
��Menu��6    $��24(�6,   $
��Bold"�6:   
�������$��20,10,20,30q�5�       $��Center=�65   #��[ 5�       �� 5�      $
��Auto=�6#   .��5�      �� .+�5�      �� -   5        $��Tasks��5�   %     SidebarButtonWithIconStyle#��[ 65   
  � Tag$-  'pack://application:,,,/Images/tasks.png��5�   %   ! � ToolTip$! Tasks��6S   �� 5�      -   5        $	��Pal��5�   %   #��[ 63   $+  %pack://application:,,,/Images/pal.png��5�   %   $	! Pal��6Q   �� 5�      -   5        $��	Documents��5�   %   #��[ 69   $1  +pack://application:,,,/Images/documents.png��5�   %   $! 	Documents��6W   �� 5�      -	   5        $��Calendar��5�   %   #��[ 68   $0  *pack://application:,,,/Images/calendar.png��5�   %   $! Calendar��6V   �� 5�      -
   5        $��Settings��5�   %   #��[ 68   $0  *pack://application:,,,/Images/settings.png��5�   %   $! Settings��6V   �� 5�      -   5        $��Logout��5�   %   #��[ 66   $.  (pack://application:,,,/Images/logout.png��5�   %   $! Logout��6T   5�      5�      5�      5�      5�      5�      5�      