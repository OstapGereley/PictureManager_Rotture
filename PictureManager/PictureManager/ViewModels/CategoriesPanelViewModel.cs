using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Input;
using PictureManager.ViewModels.Commands;

namespace PictureManager.ViewModels
{
    public class CategoriesPanelViewModel : ViewModelBase
    {
        private ObservableCollection<CategoryViewModel> _categories;
        private string _newCategoryName;
        private ITreeViewElementViewModel _selectedItem;
        private string _newTagName;

        private string _categoriesCachePath;

        public ObservableCollection<CategoryViewModel> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        private MainMetroWindowViewModel _mainMetroWindow;

        public string NewCategoryName
        {
            get { return _newCategoryName; }
            set
            {
                _newCategoryName = value;
                OnPropertyChanged("NewCategoryName");

                RaiseCanExecuteChanges(AddCategoryCommand as DelegateCommand);
            }
        }

        public string NewTagName
        {
            get { return _newTagName; }
            set
            {
                _newTagName = value;
                OnPropertyChanged("NewTagName");

                RaiseCanExecuteChanges(AddTagCommand as DelegateCommand);
            }
        }

        public ITreeViewElementViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                
                RaiseCanExecuteChanges(RemoveCategoryCommand as DelegateCommand);
                RaiseCanExecuteChanges(AddTagCommand as DelegateCommand);
                RaiseCanExecuteChanges(RemoveTagCommand as DelegateCommand);
                RaiseCanExecuteChanges(ActivateCategoryCommand as DelegateCommand);
            }
        }

        public CategoriesPanelViewModel(MainMetroWindowViewModel mainMetroWindow)
        {
            _mainMetroWindow = mainMetroWindow;

            _categoriesCachePath = ConfigurationManager.AppSettings["CategoriesCachePath"];

            using (var stream = new FileStream(_categoriesCachePath, FileMode.OpenOrCreate))
            {
                var serializer = new DataContractSerializer(typeof(List<CategoryViewModel>));

                try
                {
                    var categories = (List<CategoryViewModel>) serializer.ReadObject(stream);

                    Categories = new ObservableCollection<CategoryViewModel>(categories);
                }
                catch (Exception ex)
                {
                    Categories = new ObservableCollection<CategoryViewModel>();
                }
            }

            AddCategoryCommand = new DelegateCommand(AddCategory, CanAddCategory);
            RemoveCategoryCommand = new DelegateCommand(RemoveCategory, IsCategorySelected);
            AddTagCommand = new DelegateCommand(AddTag, CanAddTag);
            RemoveTagCommand = new DelegateCommand(RemoveTag, CanRemoveTag);
            ActivateCategoryCommand = new DelegateCommand(ActivateCategory, IsCategorySelected);
        }

        #region Commands
        public ICommand AddCategoryCommand { get; set; }

        private void AddCategory(object parameter)
        {
            Categories.Add(new CategoryViewModel {Name = NewCategoryName});
            NewCategoryName = null;
        }

        private bool CanAddCategory(object parameter)
        {
            if (string.IsNullOrEmpty(NewCategoryName))
                return false;

            return true;
        }

        public ICommand RemoveCategoryCommand { get; set; }

        private void RemoveCategory(object parameter)
        {
            Categories.Remove(SelectedItem as CategoryViewModel);
        }

        public ICommand AddTagCommand { get; set; }

        private void AddTag(object parameter)
        {
            var selectedCategory = SelectedItem as CategoryViewModel;

            selectedCategory.Tags.Add(new TagViewModel {Name = NewTagName, Parent = selectedCategory});

            NewTagName = null;
        }

        private bool CanAddTag(object parameter)
        {
            if (string.IsNullOrEmpty(NewTagName) || !(SelectedItem is CategoryViewModel))
                return false;

            return true;
        }

        public ICommand RemoveTagCommand { get; set; }

        private void RemoveTag(object parameter)
        {
            var selectedTag = SelectedItem as TagViewModel;

            var category = Categories.First(elem => elem.Name == selectedTag.Parent.Name);

            category.Tags.Remove(selectedTag);
        }

        private bool CanRemoveTag(object parameter)
        {
            if (SelectedItem is TagViewModel)
                return true;

            return false;
        }

        public ICommand ActivateCategoryCommand { get; set; }

        private void ActivateCategory(object parameter)
        {
            var selectedCategory = SelectedItem as CategoryViewModel;

            var orderedPictures = _mainMetroWindow.Pictures
                .OrderByDescending(p => p.Tags.Select(t1 => t1.Name.ToLower())
                    .Intersect(selectedCategory.Tags.Select(t2 => t2.Name.ToLower())).Count()).ToList();

            _mainMetroWindow.Pictures = new ObservableCollection<PictureViewModel>(orderedPictures);

            _mainMetroWindow.SelectedPicture = _mainMetroWindow.Pictures.First();
        }

        private bool IsCategorySelected(object parameter)
        {
            if (SelectedItem is CategoryViewModel)
                return true;

            return false;
        }


        #endregion

        public void SaveCategories()
        {
            if (Categories.Count == 0 && File.Exists(_categoriesCachePath))
            {
                File.Delete(_categoriesCachePath);
                return;
            }

            if (File.Exists(_categoriesCachePath))
                File.Delete(_categoriesCachePath);

            using (var stream = new FileStream(_categoriesCachePath, FileMode.OpenOrCreate))
            {
                var serializer = new DataContractSerializer(typeof(List<CategoryViewModel>));

                serializer.WriteObject(stream, Categories.ToList());
            }
        }
    }
}
