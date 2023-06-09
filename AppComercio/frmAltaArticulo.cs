﻿using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AppComercio
{
    public partial class frmAltaArticulo : Form
    {
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;
        public frmAltaArticulo()
        {
            InitializeComponent();
        }

        public frmAltaArticulo(Articulo art)
        {
            InitializeComponent();
            articulo = art;
        }

        private void frmAltaArticulo_Load(object sender, EventArgs e)
        {
            try
            {
                MarcaNegocio marcaNegocio = new MarcaNegocio();
                cbxMarca.DataSource = marcaNegocio.listar();
                cbxMarca.ValueMember = "Id";
                cbxMarca.DisplayMember = "Descripcion";
                CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
                cbxCategoria.DataSource = categoriaNegocio.listar();
                cbxCategoria.ValueMember = "Id";
                cbxCategoria.DisplayMember = "Descripcion";

                if (articulo != null)
                {
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    string urlImagenes = "";
                    foreach (string url in articulo.ImagenURL)
                    {
                        urlImagenes += url + ",";
                    }
                    txtUrlImagen.Text = urlImagenes;
                    cargarImagen(articulo.ImagenURL[0].ToString());
                    cbxMarca.SelectedValue = articulo.Marca.Id;
                    cbxCategoria.SelectedValue = articulo.Categoria.Id;
                    txtPrecio.Text = articulo.Precio.ToString();
                    Text = "Modificar Pokemon";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                if (validacion())
                {
                    return;
                }
                if (articulo == null)
                {
                    articulo = new Articulo();
                }
                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.Marca = (Marca)cbxMarca.SelectedItem;
                articulo.Categoria = (Categoria)cbxCategoria.SelectedItem;
                List<string> urlImagenes = txtUrlImagen.Text.Split(',').ToList();
                foreach (string palabra in urlImagenes)
                {
                    if (palabra.Contains(","))
                    {
                        urlImagenes.Remove(palabra);
                    }
                }
                articulo.ImagenURL = urlImagenes;
                articulo.Precio = decimal.Parse(txtPrecio.Text);
                ArticuloNegocio negocio = new ArticuloNegocio();

                AccesoDatos datos = new AccesoDatos();
                if (articulo.Id != 0)
                {
                    negocio.modificar(articulo);
                    MessageBox.Show("Modificado Correctamente!");
                }
                else
                {
                    negocio.agregar(articulo);
                    MessageBox.Show("Agregado Correctamente!");
                }
                 if (archivo != null && !(txtUrlImagen.Text.ToLower().Contains("http")))
                 {
                    List<string> listaFileNames = new List<string>();
                    foreach (string fileName in archivo.FileNames)
                    {
                        listaFileNames.Add(fileName);
                    }
                    List<string> listaSafeFileNames = new List<string>();
                    foreach (string safeFileName in archivo.SafeFileNames)
                    {
                        listaSafeFileNames.Add(safeFileName);
                    }
                    for (int i = 0; i < listaFileNames.Count; i++)
                    {
                        string fileName = listaFileNames[i];
                        string directorioDestino = ConfigurationManager.AppSettings["images-folder"] + listaSafeFileNames[i];
                        File.Copy(fileName, directorioDestino, true);
                    }
                 }
                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch (Exception)
            {
                pbxArticulo.Load("https://static.vecteezy.com/system/resources/previews/005/337/799/original/icon-image-not-found-free-vector.jpg");

            }
        }

        private void txtUrlImagen_TextChanged(object sender, EventArgs e)
        {
            List<string> imagenes = (txtUrlImagen.Text).Split(',').ToList();
            cargarImagen(imagenes[0]);
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "Archivos JPG (*.jpg)|*.jpg|Archivos PNG (*.png)|*.png";
            archivo.Title = "Seleccione una imagen";
            archivo.Multiselect = true;
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = "";
                foreach (string item in archivo.FileNames)
                {
                    txtUrlImagen.Text += item + ",";
                }
                txtUrlImagen.Text = txtUrlImagen.Text.TrimEnd(',');

                cargarImagen(archivo.FileNames[0]);
            }
        }

        private bool validacion()
        {
            if (txtCodigo.Text == "")
            {
                MessageBox.Show("El campo codigo esta vacio", "Error en el ingreso de datos");
                return true;
            }
            if (txtNombre.Text == "")
            {
                MessageBox.Show("El campo Nombre esta vacio", "Error en el ingreso de datos");
                return true;
            }
            if (txtDescripcion.Text == "")
            {
                MessageBox.Show("El campo Descripcion esta vacio", "Error en el ingreso de datos");
                return true;
            }
            if (txtUrlImagen.Text == "")
            {
                MessageBox.Show("El campo Url Imagen esta vacio", "Error en el ingreso de datos");
                return true;
            }
            if (txtPrecio.Text == "")
            {
                MessageBox.Show("El campo Precio esta vacio", "Error en el ingreso de datos");
                return true;
            }
            if (!(soloNumeros(txtPrecio.Text)))
            {
                MessageBox.Show("Por favor, ingrese solo numero en el precio", "Error en el ingreso de datos");
                return true;
            }
            if (comasEntreImagenes())
            {
                MessageBox.Show("Por favor, ingrese comas entra las imagenes", "Error en el ingreso de datos");
                return true;
            }
            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                //
                if (!(char.IsNumber(caracter)) && !(caracter == '.'))
                {
                    return false;
                }
            }
            return true;
        }

        private bool comasEntreImagenes()
        {
            int contador = 0;
            foreach (char caracter in txtUrlImagen.Text)
            {
                if (caracter == ',')
                {
                    contador++;
                }
            }
            if (contador == 0)
            {
                return true;
            }

            return false;
        }

        private void txtUrlImagen_MouseClick(object sender, MouseEventArgs e)
        {
            toolTipUrlImagenes.SetToolTip(txtUrlImagen, "Por favor, ingrese las imagenes separadas por comas");
        }

        private void txtUrlImagen_MouseHover(object sender, EventArgs e)
        {
            toolTipUrlImagenes.SetToolTip(txtUrlImagen, "Por favor, ingrese las imagenes separadas por comas");
        }
    }
}
