using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ValidacijaInterfejs
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Osoba o = new Osoba();
			o.Ime = null;
			DataContext = new Osoba();
		}
	}

	public class Osoba : INotifyPropertyChanged, IDataErrorInfo
	{
		private string _ime;
		public string Ime
		{
			get => _ime;
			set
			{
				_ime = value;
				PropChange("Ime");
			}
		}

		private string _prezime;
		public string Prezime
		{
			get => _prezime;
			set
			{
				_prezime = value;
				PropChange("Prezime");
			}
		}

		private int _starost;
		public int Starost
		{
			get => _starost;
			set
			{
				_starost = value;
				PropChange("Starost");
			}
		}

		public void PropChange(string prop)
		{
			if (Validator == null)
				Validator = new FValidatorOsoba();
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}


		
		public string Error
		{
			get
			{
				if (Validator != null)
				{
					var err = Validator.Validate(this);
					if (!err.IsValid)
					{
						string greske = "";

						err.Errors.ToList().ForEach(g => greske.Concat(g.ErrorMessage + " "));

						//Linija iznad radi isto sto i ovo :) 
						/*foreach (var g in err.Errors)
						{
							greske.Concat(g.ErrorMessage + " ");
						}*/

						return err.Errors.FirstOrDefault().ErrorMessage;
					}
				}
				return null;
			}
		}

		private FValidatorOsoba Validator;// = new FValidatorOsoba();

		public void Foo()
		{
			var rez = Validator.Validate(this);
			foreach(var err in rez.Errors)
			{
				//err.PropertyName
			}
			

		}

		public string this[string columnName]
		{
			get
			{
				if (Validator != null)
				{
					var rez = Validator.Validate(this);

					foreach (var err in rez.Errors)
					{
						if (err.PropertyName == columnName)
							return err.ErrorMessage;
					}	
				}
				return null;
			}
		}

		/*public string this[string columnName]
		{
			get
			{
				if (columnName == "Ime")
				{
					if (Ime != null && Ime.Count() < 4)
					{
						return "Vise od 3 karaktera!";
					}
					else if (Ime != null && Ime.Count() > 20)
						return "Manje od 20!";
				}

				if (columnName == "Starost")
				{
					if (Starost < 1)
						return "Nepravilno!";
				}
				
				return null;
			}
		}*/





		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ValidatorZaIme : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is string i)
			{
				if (string.IsNullOrEmpty(i))
					return new ValidationResult(false, "Null ili prazan :/");
				else if (i.Count() < 3)
					return new ValidationResult(false, "Premalen :/");
				else if (i.Count() > 50)
					return new ValidationResult(false, "Prevelik :/");
				else
					return ValidationResult.ValidResult;
				
			}
			else
			{
				return new ValidationResult(false, "Nije string :/");
			}
		}
	}

	public class FValidatorOsoba : AbstractValidator<Osoba>
	{
		public FValidatorOsoba()
		{
			RuleFor(o => o.Ime).NotNull().WithMessage("Prazno")
				.Length(3, 50).WithMessage("Losa duzina");
			RuleFor(o => o.Starost).GreaterThan(0).WithMessage("Nope!");
			RuleFor(o => o.Prezime).Must(Test);
		}

		public bool Test(string x) => !string.IsNullOrEmpty(x);
				
	}
}
