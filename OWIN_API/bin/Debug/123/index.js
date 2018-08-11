var app = new Vue({
	el:'#app',
	data:{
		message: 'Hello Vue!'
		}
	})
	
var app2 = new Vue({
	el:'#app-2',
	data:{
		message: '页面加载于'+new Date().toLocaleString(),
		seen:true
	}
})

var app3 = new Vue({
	el:'#app-3',
	data:{
		todos:[
		{text:'学习 JavaSc'},
			{text:'学习 Vue'},
				{text:'学习整个项目'}
		]
	}
})

var app4 = new Vue({
	el:'#app-4',
	data:{
		message:'Hello Vue.js!'
	},
	methods:{
		reverseMessage:function(){
			this.message = this.message.split('').reverse().join('')
		}
	}
})

Vue.component('todo-item',{
	props:['todo'],
	template:'<li>{{todo.text}}</li>'
})

var app5=new Vue({
	el:'#app-5',
	data:{
		groceryList:[
			{ id:0, text:'蔬菜'},
			{id:1,text:'奶酪'},
			{id:2,text:'随便吃的'}
		]
	}
})

var Main1 = {
    data() {
      return {
        tableData: [{
          date: '2016-05-02',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1518 弄'
        }, {
          date: '2016-05-04',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1517 弄'
        }, {
          date: '2016-05-01',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1519 弄'
        }, {
          date: '2016-05-03',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1516 弄'
        }],
        currentRow: null
      }
    },

    methods: {
      setCurrent(row) {
        this.$refs.singleTable.setCurrentRow(row);
      },
	  reflash() {
      	this.tableData= [{
          date: '2016-05-02',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1518 弄'
        }, {
          date: '2016-05-04',
          name: '王小虎',
          address: '上海市普陀区金沙江路 1517 弄'
        }];
      },
      handleCurrentChange(val) {
        this.currentRow = val;
      }
    }
  }
var Ctor = Vue.extend(Main1)
new Ctor().$mount('#app-61')

var vm = new Vue({
	el:'#computed',
	data:{
		message:'Hello'
	},
	computed:{
		reversedMessage:function(){
			return this.message.split('').reverse().join('')
		}
	}
})

var placeholders={"name":"请输入查找姓名","username":"请输入查找用户名","phone":"请输入查找电话"};
function getuuid(){
    var uid = [];
    var hexDigits = "0123456789abcdefghijklmnopqrst";
    for (var i = 0; i < 32; i++) {
        uid[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    uid[6] = "4";
    uid[15] = hexDigits.substr((uid[15] & 0x3) | 0x8, 1);
    var uuid = uid.join("");
    return uuid;
}
Vue.component('app111',{
  data: function() {

      var validatePass = (rule, value, callback) => {
          if (value === '') {
              callback(new Error('请再次输入密码'));
          } else if (value !== this.create.password) {
              callback(new Error('两次输入密码不一致!'));
          } else {
              callback();
          }
      };

      return {
          url: 'url',
          users: [
          ],
          create: {
              id: '',
              username: '',
              name: '',
              password: '',
              checkpass: '',
              phone: '',
              email: '',
              is_active: true
          },
          currentId:'',
          update:{
              name: '',
              phone: '',
              email: '',
              is_active: true
          },
          rules: {
              name: [
                  { required: true, message: '请输入姓名', trigger: 'blur' },
                  { min: 3, max: 15, message: '长度在 3 到 15 个字符'}
              ],
              username: [
                  { required: true, message: '请输入用户名', trigger: 'blur' },
                  { min: 3, max: 25, message: '长度在 3 到 25 个字符'},
                  { pattern:/^[A-Za-z0-9]+$/, message: '用户名只能为字母和数字'}
              ],
              password: [
                  { required: true, message: '请输入密码', trigger: 'blur' },
                  { min: 6, max: 25, message: '长度在 6 到 25 个字符'}
              ],
              checkpass: [
                  { required: true, message: '请再次输入密码', trigger: 'blur' },
                  { validator: validatePass}
              ],
              email: [
                  { type: 'email', message: '邮箱格式不正确'}
              ],
              phone:[
                  { pattern:/^1[34578]\d{9}$/, message: '请输入中国国内的手机号码'}
              ]
          },
          updateRules: {
              name: [
                  { required: true, message: '请输入姓名', trigger: 'blur' },
                  { min: 3, max: 15, message: '长度在 3 到 15 个字符'}
              ],
              email: [
                  { type: 'email', message: '邮箱格式不正确'}
              ],
              phone:[
                  { pattern:/^1[34578]\d{9}$/, message: '请输入中国国内的手机号码'}
              ]
          },
          filter: {
              per_page: 10, // 页大小
              page: 1, // 当前页
              name:'',
              username:'',
              phone:'',
              sorts:''
          },
          total_rows: 0,
          keywords: '', //搜索框的关键字内容
          select: 'username', //搜索框的搜索字段
          loading: true,
          selected: [], //已选择项
          dialogCreateVisible: false, //创建对话框的显示状态
          dialogUpdateVisible: false, //编辑对话框的显示状态
          createLoading: false,
          updateLoading: false,
          placeholder:placeholders["username"]
      };
  },
  mounted: function() {
      this.getUsers();
  },
  methods: {
      tableSelectionChange(val) {
          this.selected = val;
      },
      tableSortChange(val) {
          console.log(`排序属性: ${val.prop}`);
          console.log(`排序: ${val.order}`);
          if(val.prop!=null){
              if(val.order=='descending'){
                  this.filter.sorts = '-'+val.prop;
              }
              else{
                  this.filter.sorts = ''+val.prop;
              }
          }
          else{
              this.filter.sorts = '';
          }
          this.getUsers();
      },
      searchFieldChange(val) {
          this.placeholder=placeholders[val];
          console.log(`搜索字段： ${val} `);
      },
      pageSizeChange(val) {
          console.log(`每页 ${val} 条`);
          this.filter.per_page = val;
          this.getUsers();
      },
      pageCurrentChange(val) {
          console.log(`当前页: ${val}`);
          this.filter.page = val;
          this.getUsers();
      },
      setCurrent(user){
          this.currentId=user.id;
          this.update.name=user.name;
          this.update.phone=user.phone;
          this.update.email=user.email;
          this.update.is_active=user.is_active;
          this.dialogUpdateVisible=true;
      },
      // 重置表单
      reset() {
          this.$refs.create.resetFields();
      },
      query(){
          this.filter.name='';
          this.filter.username='';
          this.filter.phone='';
          this.filter[this.select]=this.keywords;
          this.getUsers();
      },
      // 获取用户列表
      getUsers() {
          this.loading = true;

          var resource = this.$resource(this.url);
          resource.query(this.filter)
              .then((response) => {
              this.users = response.data.datas;
              this.total_rows = response.data.total_rows;
              this.loading = false;
              this.selected.splice(0,this.selected.length);
          })
          .catch((response)=> {
                  this.$message.error('错了哦，这是一条错误消息');
              this.loading = false;
          });

      },

      // 创建用户
      createUser(){
          // 主动校验
          this.$refs.create.validate((valid) => {
              if (valid) {
                  this.create.id=getuuid();
                  this.createLoading=true;
                  var resource = this.$resource(this.url);
                  resource.save(this.create)
                      .then((response) => {
                      this.$message.success('创建用户成功！');
                      this.dialogCreateVisible=false;
                      this.createLoading=false;
                      this.reset();
                      this.getUsers();
              })
              .catch((response) => {
                      var data=response.data;
                      if(data instanceof Array){
                          this.$message.error(data[0]["message"]);
                      }
                      else if(data instanceof Object){
                          this.$message.error(data["message"]);
                      }
                      this.createLoading=false;
                  });
              }
              else {
              return false;
              }
          });
      },

      // 更新用户资料
      updateUser() {
          this.$refs.update.validate((valid) => {
              if (valid) {
                  this.updateLoading=true;
                  var actions = {
                      update: {method: 'patch'}
                  }
                  var resource = this.$resource(this.url,{},actions);
                  resource.update({"ids":this.currentId},this.update)
                      .then((response) => {
                      this.$message.success('修改用户资料成功！');
                      this.dialogUpdateVisible=false;
                      this.updateLoading=false;
                      this.getUsers();
              })
              .catch((response) => {
                  var data=response.data;
                  console.log(data);
                  if(data instanceof Array){
                      this.$message.error(data[0]["message"]);
                  }
                  else if(data instanceof Object){
                      this.$message.error(data["message"]);
                  }
                  this.updateLoading=false;
              });
              }
              else {
                  return false;
              }
          });
      },

      // 删除单个用户
      removeUser(user) {
          this.$confirm('此操作将永久删除用户 ' + user.username + ', 是否继续?', '提示', { type: 'warning' })
              .then(() => {
              // 向请求服务端删除
              var resource = this.$resource(this.url + "{/id}");
              resource.delete({ id: user.id })
                  .then((response) => {
                  this.$message.success('成功删除了用户' + user.username + '!');
                  this.getUsers();
              })
              .catch((response) => {
                      this.$message.error('删除失败!');
               });
          })
          .catch(() => {
              this.$message.info('已取消操作!');
          });
      },
      //删除多个用户
      removeUsers() {
          this.$confirm('此操作将永久删除 ' + this.selected.length + ' 个用户, 是否继续?', '提示', { type: 'warning' })
              .then(() => {
              console.log(this.selected);
          var ids = [];
          //提取选中项的id
          $.each(this.selected,(i, user)=> {
                  ids.push(user.id);
              });
          // 向请求服务端删除
          var resource = this.$resource(this.url);
          resource.remove({ids: ids.join(",") })
              .then((response) => {
                  this.$message.success('删除了' + this.selected.length + '个用户!');
                  this.getUsers();
          })
          .catch((response) => {
                  this.$message.error('删除失败!');
          });
      })
      .catch(() => {
          this.$Message('已取消操作!');
      });
      }
  },
  template: '#ClientsCRUD'
  })

new Vue({el:'#comp-test'})