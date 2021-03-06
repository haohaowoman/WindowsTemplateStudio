﻿Imports wts.ItemName.Services
Imports wts.ItemName.Helpers
Imports System.Collections.ObjectModel
Imports System.Linq
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Namespace Views
    Public NotInheritable Partial Class ShellPage
        Inherits Page
        Implements INotifyPropertyChanged

        Private Const PanoramicStateName As String = "PanoramicState"
        Private Const WideStateName As String = "WideState"
        Private Const NarrowStateName As String = "NarrowState"
        Private Const WideStateMinWindowWidth As Double = 640
        Private Const PanoramicStateMinWindowWidth As Double = 1024

        Private _isPaneOpen As Boolean
        Public Property IsPaneOpen() As Boolean
            Get
                Return _isPaneOpen
            End Get
            Set
                [Set](_isPaneOpen, value)
            End Set
        End Property

        Private _displayMode As SplitViewDisplayMode = SplitViewDisplayMode.CompactInline
        Public Property DisplayMode() As SplitViewDisplayMode
            Get
                Return _displayMode
            End Get
            Set
                [Set](_displayMode, value)
            End Set
        End Property

        Private _lastSelectedItem As Object

        Private _primaryItems As New ObservableCollection(Of ShellNavigationItem)()
        Public Property PrimaryItems() As ObservableCollection(Of ShellNavigationItem)
            Get
                Return _primaryItems
            End Get
            Set
                [Set](_primaryItems, value)
            End Set
        End Property

        Private _secondaryItems As New ObservableCollection(Of ShellNavigationItem)()
        Public Property SecondaryItems() As ObservableCollection(Of ShellNavigationItem)
            Get
                Return _secondaryItems
            End Get
            Set
                [Set](_secondaryItems, value)
            End Set
        End Property

        Public Sub New()
            Me.InitializeComponent()
            DataContext = Me
            Initialize()
        End Sub

        Private Sub Initialize()
            NavigationService.Frame = shellFrame
            AddHandler NavigationService.Frame.Navigated, AddressOf Frame_Navigated
            PopulateNavItems()

            InitializeState(Window.Current.Bounds.Width)
        End Sub

        Private Sub InitializeState(windowWith As Double)
            If windowWith < WideStateMinWindowWidth Then
                GoToState(NarrowStateName)
            ElseIf windowWith < PanoramicStateMinWindowWidth Then
                GoToState(WideStateName)
            Else
                GoToState(PanoramicStateName)
            End If
        End Sub

        Private Sub PopulateNavItems()
            _primaryItems.Clear()
            _secondaryItems.Clear()

            ' More on Segoe UI Symbol icons: https://docs.microsoft.com/windows/uwp/style/segoe-ui-symbol-font
            ' Edit String/en-US/Resources.resw: Add a menu item title for each page
        End Sub

        Private Sub Frame_Navigated(sender As Object, e As NavigationEventArgs)
            Dim navigationItem = Nothing
            
            If PrimaryItems IsNot Nothing
                navigationitem = PrimaryItems.FirstOrDefault(Function(i as ShellNavigationItem) i.PageType.Equals(e.SourcePageType))
            End If
            
            If navigationItem Is Nothing And SecondaryItems IsNot Nothing Then
                navigationItem = SecondaryItems.FirstOrDefault(Function(i as ShellNavigationItem) i.PageType.Equals(e.SourcePageType))
            End If

            If navigationItem IsNot Nothing Then
                ChangeSelected(_lastSelectedItem, navigationItem)
                _lastSelectedItem = navigationItem
            End If
        End Sub

        Private Sub ChangeSelected(oldValue As Object, newValue As Object)
            If oldValue IsNot Nothing Then
                TryCast(oldValue, ShellNavigationItem).IsSelected = False
            End If
            If newValue IsNot Nothing Then
                TryCast(newValue, ShellNavigationItem).IsSelected = True
            End If
        End Sub
        
        Private Sub Navigate(item As Object)
            Dim navigationItem = TryCast(item, ShellNavigationItem)
            If navigationItem IsNot Nothing Then
                NavigationService.Navigate(navigationItem.PageType)
            End If
        End Sub

        Private Sub ItemClicked(sender As Object, e As ItemClickEventArgs)
            If DisplayMode = SplitViewDisplayMode.CompactOverlay OrElse DisplayMode = SplitViewDisplayMode.Overlay Then
                IsPaneOpen = False
            End If
            Navigate(e.ClickedItem)
        End Sub

        Private Sub OpenPane_Click(sender As Object, e As RoutedEventArgs)
            IsPaneOpen = Not _isPaneOpen
        End Sub
    
        Private Sub WindowStates_CurrentStateChanged(sender As Object, e As VisualStateChangedEventArgs)
            GoToState(e.NewState.Name)
        End Sub

        Private Sub GoToState(stateName As String)
            Select Case stateName
                Case PanoramicStateName
                    DisplayMode = SplitViewDisplayMode.CompactInline
                    Exit Select
                Case WideStateName
                    DisplayMode = SplitViewDisplayMode.CompactInline
                    IsPaneOpen = False
                    Exit Select
                Case NarrowStateName
                    DisplayMode = SplitViewDisplayMode.Overlay
                    IsPaneOpen = False
                    Exit Select
                Case Else
                    Exit Select
            End Select
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private Sub [Set](Of T)(ByRef storage As T, value As T, <CallerMemberName> Optional propertyName As String = Nothing)
            If Equals(storage, value) Then
                Return
            End If

            storage = value
            OnPropertyChanged(propertyName)
        End Sub

        Private Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Namespace
