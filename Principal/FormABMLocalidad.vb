﻿
Public Class FormABMLocalidad
    Private _repositorio As New CapaLogica.LocalidadCLog
    Private _editForm As New FormLocalidad

    Private Sub MenuClick(ByVal sender As Object, ByVal e As EventArgs)
        Select Case DirectCast(sender, ToolStripButton).Name
            Case "BtnNuevo" : Nuevo()
            Case "BtnSalir" : Salir()
            Case "BtnImprimir" : Imprimir()
            Case "BtnEliminar" : Eliminar()
            Case "BtnEditar" : Editar()
            Case "BtnFiltrar" : Filtrar()
            Case "BtnBorrarFiltro" : BorrarFiltros()
        End Select
    End Sub

    Private Sub BorrarFiltros()
        Me.FOLVRegistros.ResetColumnFiltering()
    End Sub

    Private Sub Nuevo()
        _editForm.Entidad = Nothing

        If _editForm.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
            PoblarGrilla()
            'Me.FOLVRegistros.RefreshObject(_editForm.Entidad)
            'ActualizarEstados()
        End If

    End Sub

    Private Sub Editar()
        If Not FOLVRegistros.SelectedObject Is Nothing Then

            _editForm.Entidad = DirectCast(Me.FOLVRegistros.SelectedObject, Entidades.Localidad)

            If _editForm.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                PoblarGrilla()
                'Me.FOLVRegistros.RefreshObject(_editForm.Entidad)
                'ActualizarEstados()
            End If

        Else

            MessageBox.Show("Debe seleccionar un registro", "Eliminar registro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End If

    End Sub

    Private Sub Eliminar()

        If Not FOLVRegistros.SelectedObject Is Nothing Then

            If MessageBox.Show("Desea eliminar el registro seleccionado?", "Eliminar registro", _
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question, _
                               MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then

                'Elimino el objeto seleccionado
                _repositorio.Delete(DirectCast(FOLVRegistros.SelectedObject, Entidades.Localidad).Id)

                If _repositorio.HasError Then

                    MessageBox.Show(_repositorio.mensajes.ToString, "Eliminar registro", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Else

                    FOLVRegistros.RemoveObject(FOLVRegistros.SelectedObject)

                    ActualizarEstados()

                    MessageBox.Show("El registro se ha eliminado", "Eliminar registro", MessageBoxButtons.OK, MessageBoxIcon.Information)

                End If

            End If

        Else

            MessageBox.Show("Debe seleccionar un registro", "Eliminar registro", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End If

    End Sub

    Private Sub Salir()
        Me.Close()
    End Sub

    Private Sub Imprimir()
        Try

            If FOLVRegistros.SelectedObject Is Nothing Then
                Throw New Exception("Debe seleccionar un registro para visualizar")
            End If

            Dim r As New GeneradorReportes.Reporte

            r.SourceFile = My.Settings.RutaReportes & "\lstLocalidades.rdl"
            r.Nombre = "Listado de Localidades"

            Dim repoparametros As New CapaLogica.ParametroCLog

            r.Parametros.Add(New GeneradorReportes.Parametro("titulo", r.Nombre))

            r.Parametros.Add(New GeneradorReportes.Parametro("cdesde", DirectCast(FOLVRegistros.SelectedObject, Entidades.Localidad).Codigopostal))
            r.Parametros.Add(New GeneradorReportes.Parametro("chasta", DirectCast(FOLVRegistros.SelectedObject, Entidades.Localidad).Codigopostal))
            'r.Parametros.Add(New GeneradorReportes.Parametro("pdesde", DirectCast(FOLVRegistros.SelectedObject, Entidades.Localidad).Pcia))
            'r.Parametros.Add(New GeneradorReportes.Parametro("phasta", DirectCast(FOLVRegistros.SelectedObject, Entidades.Localidad).Pcia))

            SetearEncabezadosReporte(r)

            r.ShowReport()

        Catch ex As Exception
            MessageBox.Show("Se produjo el siguiente error: " & ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Filtrar()
        If Me.TextBoxFilter.TextLength = 0 Then
            BorrarFiltros()
            Me.TextBoxFilter.Focus()
        Else
            'aplico el filtro
            Dim filter As BrightIdeasSoftware.TextMatchFilter = BrightIdeasSoftware.TextMatchFilter.Contains(FOLVRegistros, TextBoxFilter.Text)
            FOLVRegistros.ModelFilter = filter
            FOLVRegistros.DefaultRenderer = New BrightIdeasSoftware.HighlightTextRenderer(filter)
        End If

        ActualizarEstados()
    End Sub


    Private Sub InicializarFormulario()
        ConfigurarGrilla()
        PoblarGrilla()
    End Sub

    Private Sub ConfigurarGrilla()
        CrearColumnas()

        With FOLVRegistros
            .FullRowSelect = True
            .UseFiltering = True
        End With

    End Sub

    Private Sub PoblarGrilla()
        Me.FOLVRegistros.Objects = _repositorio.GetAll.OrderBy(Function(x) x.Nombre).ToList()
        Me.FOLVRegistros.AutoResizeColumns()
        ActualizarEstados()
    End Sub

    Private Sub ActualizarEstados()
        Me.StatusLabelRegistros.Text = "Nº de Registros: " & Me.FOLVRegistros.Items.Count
    End Sub

    Private Sub CrearColumnas()

        Dim c As BrightIdeasSoftware.OLVColumn

        With FOLVRegistros

            .Columns.Clear()

            c = New BrightIdeasSoftware.OLVColumn
            c.Text = "Id"
            c.AspectName = "Id"
            c.MinimumWidth = 0
            c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
            .Columns.Add(c)

            c = New BrightIdeasSoftware.OLVColumn
            c.Text = "Código"
            c.AspectName = "Codigopostal"
            c.MinimumWidth = 50
            c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
            .Columns.Add(c)

            c = New BrightIdeasSoftware.OLVColumn
            c.Text = "Localidad"
            c.AspectName = "Nombre"
            c.MinimumWidth = 150
            c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
            c.FillsFreeSpace = True
            .Columns.Add(c)

            c = New BrightIdeasSoftware.OLVColumn
            c.Text = "Provincia"
            c.AspectName = "EntPcia"
            c.AspectToStringConverter = Function(x As Entidades.Provincias)
                                            Return x.ToString
                                        End Function
            c.MinimumWidth = 200
            c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
            .Columns.Add(c)

        End With
    End Sub

    Public Sub New()

        ' Llamada necesaria para el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        AddHandler Me.BtnNuevo.Click, AddressOf MenuClick
        AddHandler Me.BtnSalir.Click, AddressOf MenuClick
        AddHandler Me.BtnImprimir.Click, AddressOf MenuClick
        AddHandler Me.BtnEliminar.Click, AddressOf MenuClick
        AddHandler Me.BtnEditar.Click, AddressOf MenuClick
        AddHandler Me.BtnBorrarFiltro.Click, AddressOf MenuClick
        AddHandler Me.BtnFiltrar.Click, AddressOf MenuClick
        AddHandler Me.Activated, AddressOf FormularioActivo
        AddHandler Me.KeyDown, AddressOf FormularioKeyDown
    End Sub

    Private Sub FormularioKeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.F2 : Nuevo()
            Case Keys.Escape : Salir()
            Case Keys.F5 : Imprimir()
            Case Keys.Delete : Eliminar()
            Case Keys.F3 : Editar()
            Case Keys.Return : SendKeys.Send("{TAB}")
        End Select
    End Sub

    Private Sub FormABMLocalidad_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InicializarFormulario()
    End Sub

    Private Sub FOLVRegistros_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles FOLVRegistros.DoubleClick
        Editar()
    End Sub

    Private Sub FormularioActivo(ByVal sender As Object, ByVal e As EventArgs)
        Me.TextBoxFilter.Focus()
    End Sub

    Private Sub TextBoxFilter_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TextBoxFilter.TextChanged
        If Len(Trim(TextBoxFilter.Text)) = 0 Then
            BorrarFiltros()
        Else
            Filtrar()
        End If
        TextBoxFilter.Focus()
    End Sub
End Class