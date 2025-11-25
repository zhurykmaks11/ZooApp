using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class ReportsWindow : Window
    {
        private readonly string _role;
        private readonly string _username;

        // Сервіси
        private readonly AnimalsService _animalsService;
        private readonly CagesService _cagesService;
        private readonly MedicalService _medicalService;
        private readonly EmployeeService _employeeService;
        private readonly SupplierService _supplierService;
        private readonly FeedingService _feedingService;
        private readonly ExchangeService _exchangeService;

        // Кешовані колекції
        private List<Animal> _allAnimals = new();
        private List<Cage> _allCages = new();
        private List<Employee> _allEmployees = new();
        private List<Supplier> _allSuppliers = new();

        public ReportsWindow(string role, string username)
        {
            InitializeComponent();

            _role = role;
            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");

            _animalsService = new AnimalsService(context);
            _cagesService = new CagesService(context);
            _medicalService = new MedicalService(context);
            _employeeService = new EmployeeService(context);
            _supplierService = new SupplierService(context);
            _feedingService = new FeedingService(context);
            _exchangeService = new ExchangeService(context);

            LoadLookups();

            // За замовчуванням — перший запит
            ReportSelector.SelectedIndex = 0;
            ShowParamsForReport("1");
        }

        #region Helpers: Load lookups, age calc

        private void LoadLookups()
        {
            _allAnimals = _animalsService.GetAllAnimals();
            _allCages = _cagesService.GetAllCages();
            _allEmployees = _employeeService.GetAllEmployees();
            _allSuppliers = _supplierService.GetAll();

            // Q1 cages
            Q1_CageCombo.ItemsSource = _allCages;

            // Q6 cages
            Q6_CageCombo.ItemsSource = _allCages;

            // Q3 animals
            Q3_AnimalCombo.ItemsSource = _allAnimals;

            // Заглушка – якщо щось не вибране, щоб не падало
            if (Q1_SortCombo.SelectedItem == null && Q1_SortCombo.Items.Count > 0)
                Q1_SortCombo.SelectedIndex = 0;
        }

        private int GetAgeYears(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private int GetYears(DateTime start)
        {
            var today = DateTime.Today;
            int years = today.Year - start.Year;
            if (start.Date > today.AddYears(-years)) years--;
            return years;
        }

        #endregion

        #region UI switching: parameters

        private void ReportSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ReportSelector.SelectedItem as ComboBoxItem;
            string tag = item?.Tag?.ToString() ?? "1";
            ShowParamsForReport(tag);
        }

        private void HideAllParamPanels()
        {
            Params_Q1.Visibility = Visibility.Collapsed;
            Params_Q2.Visibility = Visibility.Collapsed;
            Params_Q3.Visibility = Visibility.Collapsed;
            Params_Q4.Visibility = Visibility.Collapsed;
            Params_Q5.Visibility = Visibility.Collapsed;
            Params_Q6.Visibility = Visibility.Collapsed;
            Params_Q7.Visibility = Visibility.Collapsed;
            Params_Q8.Visibility = Visibility.Collapsed;
            Params_Q9.Visibility = Visibility.Collapsed;
            Params_Q10.Visibility = Visibility.Collapsed;
        }

        private void ShowParamsForReport(string tag)
        {
            HideAllParamPanels();
            SummaryText.Text = "";

            switch (tag)
            {
                case "1":
                    Params_Q1.Visibility = Visibility.Visible;
                    break;
                case "2":
                    Params_Q2.Visibility = Visibility.Visible;
                    break;
                case "3":
                    Params_Q3.Visibility = Visibility.Visible;
                    break;
                case "4":
                    Params_Q4.Visibility = Visibility.Visible;
                    break;
                case "5":
                    Params_Q5.Visibility = Visibility.Visible;
                    break;
                case "6":
                    Params_Q6.Visibility = Visibility.Visible;
                    break;
                case "7":
                    Params_Q7.Visibility = Visibility.Visible;
                    break;
                case "8":
                    Params_Q8.Visibility = Visibility.Visible;
                    break;
                case "9":
                    Params_Q9.Visibility = Visibility.Visible;
                    break;
                case "10":
                    Params_Q10.Visibility = Visibility.Visible;
                    break;
            }
        }

        #endregion

        #region Run button

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ReportSelector.SelectedItem as ComboBoxItem;
            string tag = item?.Tag?.ToString() ?? "1";

            try
            {
                switch (tag)
                {
                    case "1": RunQuery1_AnimalsGeneral(); break;
                    case "2": RunQuery2_Medical(); break;
                    case "3": RunQuery3_EmployeeAccess(); break;
                    case "4": RunQuery4_Suppliers(); break;
                    case "5": RunQuery5_FeedSeason(); break;
                    case "6": RunQuery6_AnimalsFull(); break;
                    case "7": RunQuery7_PartnerZoos(); break;
                    case "8": RunQuery8_EmployeesFilters(); break;
                    case "9": RunQuery9_WarmAnimals(); break;
                    case "10": RunQuery10_ZooFeeds(); break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка під час виконання запиту:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Query 1: Animals general

        // 1) Одержати загальну кількість та список всіх тварин у зоопарку
        //    + тварин вказаного виду, які живуть у вказаній клітці, 
        //    з можливістю сортувати за віком / вагою / зростом.
        private void RunQuery1_AnimalsGeneral()
        {
            string speciesFilter = Q1_SpeciesBox.Text.Trim().ToLower();
            string cageId = Q1_CageCombo.SelectedValue?.ToString();
            string sortTag = (Q1_SortCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "none";

            var animals = _allAnimals.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(speciesFilter))
            {
                animals = animals.Where(a =>
                    !string.IsNullOrEmpty(a.Species) &&
                    a.Species.ToLower().Contains(speciesFilter));
            }

            if (!string.IsNullOrEmpty(cageId))
            {
                animals = animals.Where(a => a.CageId == cageId);
            }

            var cageDict = _allCages.ToDictionary(c => c.IdString, c => c.Location);

            var result = animals
                .Select(a => new
                {
                    a.Name,
                    a.Species,
                    a.Gender,
                    Age = GetAgeYears(a.BirthDate),
                    a.Weight,
                    a.Height,
                    Cage = (a.CageId != null && cageDict.ContainsKey(a.CageId))
                        ? cageDict[a.CageId]
                        : "(немає клітки)"
                })
                .ToList();

            switch (sortTag)
            {
                case "age":
                    result = result.OrderBy(r => r.Age).ToList();
                    break;
                case "weight":
                    result = result.OrderBy(r => r.Weight).ToList();
                    break;
                case "height":
                    result = result.OrderBy(r => r.Height).ToList();
                    break;
            }

            ReportsGrid.ItemsSource = result;
            SummaryText.Text = $"Кількість тварин у вибірці: {result.Count}";
        }

        #endregion

        #region Query 2: Vaccines / illnesses / offspring

        // 2) Тварини: яким зроблене вказане щеплення, які 
        //    перехворіли вказаною хворобою, за віком, статтю, потомством.
        private void RunQuery2_Medical()
        {
            string vaccineFilter = Q2_VaccineBox.Text.Trim().ToLower();
            string illnessFilter = Q2_IllnessBox.Text.Trim().ToLower();
            string genderTag = (Q2_GenderCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "any";

            int minAge = 0;
            int.TryParse(Q2_MinAgeBox.Text.Trim(), out minAge);

            int minOffspring = 0;
            int.TryParse(Q2_MinOffspringBox.Text.Trim(), out minOffspring);

            var records = _medicalService.GetAllRecords();
            var animals = _allAnimals;

            // обчислення кількості потомства
            var offspringCounts = animals.ToDictionary(a => a.Id, a =>
                animals.Count(ch => ch.MotherId == a.Id || ch.FatherId == a.Id));

            var joined = from r in records
                         join a in animals on r.AnimalId equals a.Id into gj
                         from a in gj.DefaultIfEmpty()
                         select new { Record = r, Animal = a };

            var list = new List<object>();

            foreach (var item in joined)
            {
                var a = item.Animal;
                if (a == null) continue;

                var last = item.Record.Checkups
                    .OrderByDescending(c => c.Date)
                    .FirstOrDefault();

                // фільтр по статі
                if (genderTag == "male" && a.Gender != "male") continue;
                if (genderTag == "female" && a.Gender != "female") continue;

                int age = GetAgeYears(a.BirthDate);
                if (age < minAge) continue;

                int offs = offspringCounts.ContainsKey(a.Id) ? offspringCounts[a.Id] : 0;
                if (offs < minOffspring) continue;

                if (!string.IsNullOrWhiteSpace(vaccineFilter))
                {
                    if (last == null ||
                        last.Vaccinations == null ||
                        !last.Vaccinations.Any(v =>
                            v != null && v.ToLower().Contains(vaccineFilter)))
                        continue;
                }

                if (!string.IsNullOrWhiteSpace(illnessFilter))
                {
                    if (last == null ||
                        last.Illnesses == null ||
                        !last.Illnesses.Any(i =>
                            i != null && i.ToLower().Contains(illnessFilter)))
                        continue;
                }

                list.Add(new
                {
                    Animal = a.Name,
                    a.Species,
                    a.Gender,
                    Age = age,
                    LastCheckup = last?.Date.ToString("dd.MM.yyyy") ?? "—",
                    Vaccinations = (last != null && last.Vaccinations != null && last.Vaccinations.Any())
                        ? string.Join(", ", last.Vaccinations)
                        : "—",
                    Illnesses = (last != null && last.Illnesses != null && last.Illnesses.Any())
                        ? string.Join(", ", last.Illnesses)
                        : "—",
                    OffspringCount = offs
                });
            }

            ReportsGrid.ItemsSource = list;
            SummaryText.Text = $"Кількість тварин у вибірці: {list.Count}";
        }

        #endregion

        #region Query 3: Employees responsible / with access


private void RunQuery3_EmployeeAccess()
{
    string speciesFilter = Q3_SpeciesBox.Text.Trim().ToLower();
    string animalId = Q3_AnimalCombo.SelectedValue?.ToString();

    var animals = _allAnimals;
    var employees = _allEmployees;

    // Категорії, які мають доступ до тварин
    var accessCategories = new[] { "vet", "cleaner", "trainer" };

    // IDs тварин, яких шукаємо
    var targetAnimalIds = new HashSet<string>();

    // 🔍 Фільтр за видом
    if (!string.IsNullOrWhiteSpace(speciesFilter))
    {
        foreach (var a in animals.Where(a =>
                     !string.IsNullOrEmpty(a.Species) &&
                     a.Species.ToLower().Contains(speciesFilter)))
        {
            targetAnimalIds.Add(a.Id);
        }
    }

    // 🔍 Фільтр за конкретною твариною
    if (!string.IsNullOrEmpty(animalId))
    {
        targetAnimalIds.Add(animalId);
    }

    var result = new List<object>();

    if (targetAnimalIds.Count > 0)
    {
        // 📌 Вибрані тварини → шукаємо працівників, які закріплені за ними
        foreach (var emp in employees)
        {
            if (!accessCategories.Contains(emp.Category.ToLower()))
                continue;

            bool assigned =
                emp.AnimalsUnderCare != null &&
                emp.AnimalsUnderCare.Any(id => targetAnimalIds.Contains(id));

            if (assigned)
            {
                result.Add(new
                {
                    emp.FullName,
                    emp.Category,
                    emp.Gender,
                    WorkYears = GetYears(emp.WorkStartDate),
                    AnimalsUnderCare = emp.AnimalsUnderCare?.Count ?? 0
                });
            }
        }
    }
    else
    {
        // 📌 Нічого не вибрали → повертаємо всіх, хто має доступ до кліток
        foreach (var emp in employees.Where(e =>
                     accessCategories.Contains(e.Category.ToLower())))
        {
            result.Add(new
            {
                emp.FullName,
                emp.Category,
                emp.Gender,
                WorkYears = GetYears(emp.WorkStartDate),
                AnimalsUnderCare = emp.AnimalsUnderCare?.Count ?? 0
            });
        }
    }

    ReportsGrid.ItemsSource = result;
    SummaryText.Text = $"Кількість працівників: {result.Count}";
}

#endregion


        #region Query 4: Suppliers

        // 4) Постачальники кормів: загалом + фільтр по типу корму.
        private void RunQuery4_Suppliers()
        {
            string feedFilter = Q4_FeedTypeBox.Text.Trim().ToLower();

            var suppliers = _allSuppliers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(feedFilter))
            {
                suppliers = suppliers.Where(s =>
                    s.FeedTypes != null &&
                    s.FeedTypes.Any(f => f != null && f.ToLower().Contains(feedFilter)));
            }

            var list = suppliers.Select(s => new
            {
                s.Name,
                s.Address,
                s.Phone,
                FeedTypes = s.FeedTypes != null
                    ? string.Join(", ", s.FeedTypes)
                    : "—",
                FeedTypesCount = s.FeedTypes?.Count ?? 0
            }).ToList();

            ReportsGrid.ItemsSource = list;
            SummaryText.Text =
                $"Кількість постачальників у вибірці: {list.Count} (всього в системі: {_allSuppliers.Count})";
        }

        #endregion

        #region Query 5: Animals by feed & season

        // 5) Перелік та кількість тварин, які потребують певного корму
        //    у вказаному сезоні або цілий рік.
        private void RunQuery5_FeedSeason()
        {
            string feedType = Q5_FeedTypeBox.Text.Trim().ToLower();
            string seasonTag = (Q5_SeasonCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "any";

            var feedings = _feedingService.GetAllFeedings().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(feedType))
            {
                feedings = feedings.Where(f =>
                    !string.IsNullOrEmpty(f.FeedType) &&
                    f.FeedType.ToLower().Contains(feedType));
            }

            if (seasonTag != "any")
            {
                feedings = feedings.Where(f =>
                    !string.IsNullOrEmpty(f.Season) &&
                    f.Season.Equals(seasonTag, StringComparison.OrdinalIgnoreCase));
            }

            // Об'єднати по тварині + типу корму (щоб не дублюватись зайво)
            var grouped = feedings
                .GroupBy(f => new { f.AnimalName, f.FeedType, f.Season })
                .Select(g => new
                {
                    g.Key.AnimalName,
                    g.Key.FeedType,
                    g.Key.Season,
                    TotalQuantity = g.Sum(x => x.QuantityKg)
                })
                .ToList();

            ReportsGrid.ItemsSource = grouped;
            SummaryText.Text =
                $"Кількість записів (тварина+корм+сезон): {grouped.Count}";
        }

        #endregion

        #region Query 6: Full animals info + potential offspring

        // 6) Повна інформація про тварин, фільтр по виду, клітці, імені +
        //    перелік тварин, від яких можна очікувати потомства.
        private void RunQuery6_AnimalsFull()
        {
            string speciesFilter = Q6_SpeciesBox.Text.Trim().ToLower();
            string cageId = Q6_CageCombo.SelectedValue?.ToString();
            string nameFilter = Q6_NameBox.Text.Trim().ToLower();
            bool onlyPotential = Q6_OnlyPotentialParents.IsChecked == true;

            var animals = _allAnimals.AsEnumerable();
            var cagesDict = _allCages.ToDictionary(c => c.IdString, c => c.Location);

            if (!string.IsNullOrWhiteSpace(speciesFilter))
            {
                animals = animals.Where(a =>
                    !string.IsNullOrEmpty(a.Species) &&
                    a.Species.ToLower().Contains(speciesFilter));
            }

            if (!string.IsNullOrEmpty(cageId))
            {
                animals = animals.Where(a => a.CageId == cageId);
            }

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                animals = animals.Where(a =>
                    !string.IsNullOrEmpty(a.Name) &&
                    a.Name.ToLower().Contains(nameFilter));
            }

            var allAnimals = _allAnimals;

            var list = new List<object>();

            foreach (var a in animals)
            {
                int age = GetAgeYears(a.BirthDate);
                int offspring = allAnimals.Count(ch => ch.MotherId == a.Id || ch.FatherId == a.Id);

                bool potentialParent = age >= 2; // дуже проста логіка

                if (onlyPotential && !potentialParent)
                    continue;

                list.Add(new
                {
                    a.Name,
                    a.Species,
                    a.Gender,
                    Age = age,
                    a.Weight,
                    a.Height,
                    Cage = (a.CageId != null && cagesDict.ContainsKey(a.CageId))
                        ? cagesDict[a.CageId]
                        : "(немає клітки)",
                    a.Type,
                    NeedsWarmShelter = a.NeedsWarmShelter ? "так" : "ні",
                    IsIsolated = a.IsIsolated ? "так" : "ні",
                    OffspringCount = offspring,
                    PotentialParent = potentialParent ? "так" : "ні"
                });
            }

            ReportsGrid.ItemsSource = list;
            SummaryText.Text = $"Кількість тварин у вибірці: {list.Count}";
        }

        #endregion

        #region Query 7: Partner zoos

        // 7) Список та загальна кількість зоопарків, з якими відбувся обмін
        //    тваринами загалом або лише вказаного виду.
        private void RunQuery7_PartnerZoos()
        {
            string speciesFilter = Q7_SpeciesBox.Text.Trim().ToLower();

            var records = _exchangeService.GetAll();
            var animals = _allAnimals.ToDictionary(a => a.Id, a => a);

            if (!string.IsNullOrWhiteSpace(speciesFilter))
            {
                records = records
                    .Where(r =>
                    {
                        if (!animals.ContainsKey(r.AnimalId)) return false;
                        var a = animals[r.AnimalId];
                        return !string.IsNullOrEmpty(a.Species) &&
                               a.Species.ToLower().Contains(speciesFilter);
                    })
                    .ToList();
            }

            var grouped = records
                .GroupBy(r => r.OtherZoo)
                .Select(g => new
                {
                    OtherZoo = g.Key,
                    ExchangesCount = g.Count()
                })
                .OrderByDescending(x => x.ExchangesCount)
                .ToList();

            ReportsGrid.ItemsSource = grouped;
            SummaryText.Text =
                $"Кількість зоопарків-партнерів: {grouped.Count}, загальна кількість операцій обміну: {records.Count}";
        }

        #endregion

        #region Query 8: Employees filters

        // 8) Список та загальна кількість працівників: категорія, стаж,
        //    стать, вік, зарплата.
        private void RunQuery8_EmployeesFilters()
        {
            string category = (Q8_CategoryCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "(будь-яка)";
            string genderTag = (Q8_GenderCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "any";

            int minYears = 0;
            int.TryParse(Q8_MinYearsBox.Text.Trim(), out minYears);

            double minSalary = 0;
            double.TryParse(Q8_MinSalaryBox.Text.Trim(), out minSalary);

            var employees = _allEmployees.AsEnumerable();

            if (category != "(будь-яка)")
            {
                employees = employees.Where(e =>
                    e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (genderTag == "male")
                employees = employees.Where(e => e.Gender == "male");
            else if (genderTag == "female")
                employees = employees.Where(e => e.Gender == "female");

            var list = new List<object>();

            foreach (var e in employees)
            {
                int age = GetAgeYears(e.BirthDate);
                int yearsWork = GetYears(e.WorkStartDate);

                if (yearsWork < minYears) continue;
                if (e.Salary < minSalary) continue;

                list.Add(new
                {
                    e.FullName,
                    e.Category,
                    e.Gender,
                    Age = age,
                    YearsOfWork = yearsWork,
                    e.Salary
                });
            }

            ReportsGrid.ItemsSource = list;
            SummaryText.Text =
                $"Кількість працівників у вибірці: {list.Count} (всього: {_allEmployees.Count})";
        }

        #endregion

        #region Query 9: Warm animals

        // 9) Список та загальна кількість тварин, які потребують теплого приміщення
        //    на зиму, по виду / віку.
        private void RunQuery9_WarmAnimals()
        {
            string speciesFilter = Q9_SpeciesBox.Text.Trim().ToLower();
            int minAge = 0;
            int.TryParse(Q9_MinAgeBox.Text.Trim(), out minAge);

            var animals = _allAnimals
                .Where(a => a.NeedsWarmShelter)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(speciesFilter))
            {
                animals = animals.Where(a =>
                    !string.IsNullOrEmpty(a.Species) &&
                    a.Species.ToLower().Contains(speciesFilter));
            }

            var list = new List<object>();

            foreach (var a in animals)
            {
                int age = GetAgeYears(a.BirthDate);
                if (age < minAge) continue;

                list.Add(new
                {
                    a.Name,
                    a.Species,
                    a.Gender,
                    Age = age,
                    a.Type,
                    NeedsWarmShelter = "так"
                });
            }

            ReportsGrid.ItemsSource = list;
            SummaryText.Text =
                $"Кількість тварин, що потребують теплого приміщення: {list.Count}";
        }

        #endregion

        #region Query 10: Zoo feeds

        // 10) Список та обсяг кормів, які виготовлені зоопарком повністю /
        //     або кормів, якими зоопарк забезпечує себе повністю.
        //
        // У спрощеному варіанті:
        //  - групуємо FeedingSchedule по типу корму
        //  - дивимось, чи є хоч один постачальник, який постачає цей корм
        //  - якщо постачальників немає → вважаємо, що зоопарк виготовляє сам.
        private void RunQuery10_ZooFeeds()
        {
            string modeTag = (Q10_ModeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "all";

            var feedings = _feedingService.GetAllFeedings();

            var grouped = feedings
                .GroupBy(f => f.FeedType ?? "")
                .Select(g => new
                {
                    FeedType = g.Key,
                    TotalKg = g.Sum(x => x.QuantityKg)
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.FeedType))
                .ToList();

            var result = new List<object>();

            foreach (var g in grouped)
            {
                bool hasSupplier = _allSuppliers.Any(s =>
                    s.FeedTypes != null &&
                    s.FeedTypes.Any(ft =>
                        ft != null && ft.Equals(g.FeedType, StringComparison.OrdinalIgnoreCase)));

                if (modeTag == "internal" && hasSupplier)
                    continue;

                result.Add(new
                {
                    g.FeedType,
                    g.TotalKg,
                    HasSuppliers = hasSupplier ? "так" : "ні"
                });
            }

            ReportsGrid.ItemsSource = result;
            SummaryText.Text =
                $"Кількість типів кормів у вибірці: {result.Count}";
        }

        #endregion

        #region Заглушки-імена (щоб точно не було помилок типу 'Cannot resolve symbol')

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }


        private void ShowParams_EmployeeAccess() => ShowParamsForReport("3");
        private void ShowParams_Suppliers() => ShowParamsForReport("4");
        private void ShowParams_FeedSeason() => ShowParamsForReport("5");
        private void ShowParams_AnimalsFull() => ShowParamsForReport("6");
        private void ShowParams_PartnerZoos() => ShowParamsForReport("7");
        private void ShowParams_EmployeesFilters() => ShowParamsForReport("8");
        private void ShowParams_WarmAnimals() => ShowParamsForReport("9");
        private void ShowParams_ZooFeeds() => ShowParamsForReport("10");

        #endregion
    }
}
