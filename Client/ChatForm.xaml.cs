using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client {
    /// <summary>
    /// Interaction logic for ChatForm.xaml
    /// </summary>
    public partial class ChatForm : Window {
        public ChatForm() {
            InitializeComponent();

            /* INotifyCollectionChanged는 XAML에서 디자인한 폼에서 데이터의 즉각적인 변화 또는 데이터를 가져올 때 사용하는 Interface
             * INotifyCollectionChanged는 단일 데이터의 변경을 감지하는 것이고, 리스트처럼 다수의 데이터 조작이 필요할 경우,
             * 해당 인터페이스를 적용한 후, ObservableCollection<T> 클래스를 이용하여 데이터를 삽입해야 함
             * 
             * .CollectionChanged는 리스트 객체의 멤버(Items)에 변화가 발생되는 경우를 트리거하여 실행되는 이벤트
             * ObservableCollection<T>
             */
            ((INotifyCollectionChanged)ChatList.Items).CollectionChanged += Messages_CollectionChanged;
        }

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems == null) return;     // 추가된 객체가 null일 경우, 취소

            // ListView 아이템의 표시(Displaying)되는 스크롤 위치를 마지막 아이템의 인덱스(ChatList.Items.Count -1)로 이동
            if (e.NewItems.Count > 0) ChatList.ScrollIntoView(ChatList.Items[ChatList.Items.Count - 1]);
        }
    }
}
