using MySignTool.Models.Interfaces;
using Prism.Common;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MySignTool.Models.SignAlgorithms;
using System.Linq;
using Prism.Commands;
using MySignTool.Models.Keys;
using System.Windows;
using Microsoft.Win32;
using MySignTool.Models.Hash;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace MySignTool.ViewModels
{
    class SignToolMainVM : BindableBase
    {
        private ISignature _selectedAlgorithm;

        private List<IKey> _keys;

        private IKey _key = default;

        public IKey Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                RaisePropertyChanged(nameof(Key));
            }
        }
        public ISignature SelectedAlgorithm
        {
            get
            {
                return _selectedAlgorithm;
            }
            set
            {
                _selectedAlgorithm = value;
                Key = _keys.Find((k) => k.Name == _selectedAlgorithm.Name);
                RaisePropertyChanged(nameof(SelectedAlgorithm));
            }
        }
        public ObservableCollection<ISignature> SigningAlgorithms { get; set; }
        public DelegateCommand GenerateKey { get; }
        public DelegateCommand ShowKey { get; }
        public DelegateCommand VerifySignature { get; }
        public DelegateCommand Sign { get; }

        public DelegateCommand WriteKeyToFile { get; }

        public DelegateCommand LoadKeyFromFile { get; }
        public SignToolMainVM()
        {
            #region add signature and keys to list

            SigningAlgorithms = new ObservableCollection<ISignature>
            {
                new RSADigitalSignClass()
            };
            _keys = new List<IKey>
            {
                new RsaKey()
            };
            RaisePropertyChanged(nameof(SigningAlgorithms));
            SelectedAlgorithm = SigningAlgorithms.First();

            #endregion

            GenerateKey = new DelegateCommand(() =>
            {
                try
                {
                    Key.GenerateKey();
                    RaisePropertyChanged(nameof(Key));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });

            ShowKey = new DelegateCommand(() =>
            {
                try
                {
                    if (Key.IsKeyEmpty())
                    {
                        throw new ApplicationException("Firstly, generate keys");
                    }
                    MessageBox.Show($"Key Info:\nGeneral Parameter: {Key.GeneralParameter}\nOpen Parameter: {Key.OpenParameter}\nSecret Parameter: {Key.SecretParameter}");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
            
            Sign = new DelegateCommand(() =>
            {
                try
                {
                    if (Key.IsKeyEmpty())
                    {
                        throw new ApplicationException("Firstly, generate keys");
                    }
                    OpenFileDialog ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == true)
                    {
                        string filePath = ofd.FileName;
                        string hash = SHA1.GetHash(filePath);
                        byte[] signature = SelectedAlgorithm.Sign(Key, hash);
                        filePath = Path.GetDirectoryName(filePath) + @"signature";
                        using FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                        fs.Write(signature);
                    } 
                    else
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });

            VerifySignature = new DelegateCommand(() =>
            {
                try
                {
                    if (Key.IsKeyEmpty())
                    {
                        throw new ApplicationException("Firstly, generate keys");
                    }
                    OpenFileDialog ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == true)
                    {
                        string filePath = ofd.FileName;
                        string hash = SHA1.GetHash(filePath);
                        byte[] signature = default;

                        if (ofd.ShowDialog() == true)
                        {
                            filePath = ofd.FileName;
                            using FileStream fs = new FileStream(filePath, FileMode.Open);
                            signature = new byte[fs.Length];
                            fs.Read(signature);
                        }
                        if (signature == default)
                        {
                            return;
                        }
                        bool isSignatureValid = SelectedAlgorithm.VerifySignature(hash, signature, Key);
                        if (isSignatureValid)
                        {
                            MessageBox.Show("Signature is valid");
                        }
                        else
                        {
                            MessageBox.Show("Signature is not valid");
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });

            WriteKeyToFile = new DelegateCommand(() =>
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                if (Key.IsKeyEmpty())
                {
                    throw new ApplicationException("Firstly, generate keys");
                }
                dialog.InitialDirectory = "C:\\Users";
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    KeyWorkerClass.WriteKeyToFile(Key, dialog.FileName);
                }
                else
                {
                    return;
                }
            });

            LoadKeyFromFile = new DelegateCommand(() =>
            {
                try
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == true)
                    {
                        string filePath = ofd.FileName;
                        GeneralKeyType key = KeyWorkerClass.LoadKeyFromFile(filePath);
                        IKey loadedKey = _keys.Find((k) => k.Name == _selectedAlgorithm.Name);
                        string type = loadedKey.LoadKey(key);
                        MessageBox.Show($"{type} key is loaded successfully!");
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }
    }
}
